using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using NickTax.Domain.Interfaces;
using NickTax.Domain.Models;

namespace NickTax.Infrastructure.Services
{
    public class FocusNfeClient : IFocusClient
    {
        private readonly HttpClient _httpClient;
        private const string TokenFocus = "SEU_TOKEN_DA_FOCUS_NFE";

        public FocusNfeClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://homologacao.focusnfe.com.br");

            var authenticationString = $"{TokenFocus}:";
            var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        }

        public async Task<bool> CadastrarEmpresaAsync(string cnpj, string razaoSocial, byte[] certificadoBytes, string senha)
        {
            string certificadoBase64 = Convert.ToBase64String(certificadoBytes);

            // Montando o payload exatamente com os nós obrigatórios exigidos pela Focus NFe
            var dadosEmpresa = new
            {
                nome = razaoSocial,
                razao_social = razaoSocial,
                cnpj = cnpj,
                inscricao_estadual = "ISENTO",
                regime_tributario = 1, // 1 = Simples Nacional
                email = "seuemail@exemplo.com",
                telefone = "12999999999",

                // Blocos obrigatórios de endereço
                logradouro = "Rua Alberto Cintra",
                numero = "100",
                bairro = "Centro",
                municipio = "São José dos Campos",
                uf = "SP",
                cep = "12200000",

                // Envio do arquivo e senha
                certificado = certificadoBase64,
                certificado_senha = senha,

                // Habilita o robô de leitura de notas recebidas
                habilita_nfe_recebida = true
            };

            var json = JsonSerializer.Serialize(dadosEmpresa);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Adicionado ?dry_run=1 na rota para simulação segura
            var response = await _httpClient.PostAsync("/v2/empresas?dry_run=1", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DispararPesquisaNotasAsync(string cnpj)
        {
            // Dizemos para a Focus iniciar uma busca de manifestos/notas recebidas para este CNPJ
            var dadosPesquisa = new { cnpj = cnpj };
            var json = JsonSerializer.Serialize(dadosPesquisa);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/v2/vendas/manifestos", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<FocusQueryResult> ConsultarNotasAsync(string cnpj, long ultimoNSU)
        {
            try
            {
                // Coleta as notas armazenadas na Focus a partir do último NSU salvo no seu banco
                var response = await _httpClient.GetAsync($"/v2/vendas/manifestos?cnpj={cnpj}&nsu={ultimoNSU}");

                if (!response.IsSuccessStatusCode)
                {
                    return new FocusQueryResult(false, $"Erro Focus: HTTP {response.StatusCode}", ultimoNSU, new List<string>());
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonResult);
                var root = doc.RootElement;

                long maxNsu = ultimoNSU;
                var loteXmls = new List<string>();

                if (root.TryGetProperty("manifestos", out var manifestosArray))
                {
                    foreach (var manifesto in manifestosArray.EnumerateArray())
                    {
                        if (manifesto.TryGetProperty("nsu", out var nsuProp) && nsuProp.TryGetInt64(out var nsuValor))
                        {
                            if (nsuValor > maxNsu) maxNsu = nsuValor;
                        }

                        if (manifesto.TryGetProperty("xml", out var xmlProp))
                        {
                            loteXmls.Add(xmlProp.GetString() ?? string.Empty);
                        }
                    }
                }

                if (loteXmls.Count == 0)
                {
                    return new FocusQueryResult(true, "Nenhum manifesto novo encontrado no painel.", maxNsu, loteXmls);
                }

                return new FocusQueryResult(true, "Consulta realizada com sucesso.", maxNsu, loteXmls);
            }
            catch (Exception ex)
            {
                return new FocusQueryResult(false, $"Falha de conexão: {ex.Message}", ultimoNSU, new List<string>());
            }
        }
    }
}