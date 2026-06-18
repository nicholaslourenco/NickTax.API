using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

using NickTax.Application.DTOs;

namespace NickTax.Tests;

public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(WebApplicationFactory<Program> factory)
    {
        // Cria o cliente HTTP apontando para o servidor em memória da API
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task FluxoAutenticacao_DeveRegistrar_Logar_E_AcessarRotaProtegidaComSucesso()
    {
        // Arrumar (Arrange) - Dados de teste únicos para evitar conflito no banco
        var emailUnico = $"teste_{Guid.NewGuid()}@nicktax.com";
        var registroDto = new CreateUsuarioRequest("Usuario Teste", emailUnico, "SenhaForte123!");
        var loginDto = new LoginRequest(emailUnico, "SenhaForte123!");

        // 1. Agir (Act) - Registrar Usuário
        var responseRegistro = await _client.PostAsJsonAsync("api/usuarios/register", registroDto);

        // Assertivas do Registro
        Assert.Equal(HttpStatusCode.OK, responseRegistro.StatusCode);

        // 2. Agir - Fazer Login para obter o Token JWT
        var responseLogin = await _client.PostAsJsonAsync("api/auth/login", loginDto);
        Assert.Equal(HttpStatusCode.OK, responseLogin.StatusCode);

        var resultadoLogin = await responseLogin.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(resultadoLogin);
        Assert.False(string.IsNullOrEmpty(resultadoLogin.Token));

        // 3. Agir - Tentar acessar uma rota protegida SEM o token (Deve dar 411 Unauthorized)
        var responseInvalida = await _client.GetAsync("api/usuarios/me"); // Mude para alguma rota [Authorize] que você tenha
        Assert.Equal(HttpStatusCode.Unauthorized, responseInvalida.StatusCode);

        // 4. Agir - Injetar o Token JWT no cabeçalho e tentar acessar a rota protegida novamente
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", resultadoLogin.Token);

        var responseValida = await _client.GetAsync("api/usuarios/me");

        // Assertiva Final - Deve deixar passar com sucesso (200 OK)
        Assert.Equal(HttpStatusCode.OK, responseValida.StatusCode);
    }
}