using Microsoft.AspNetCore.Mvc;
using NickTax.Application.DTOs;
using NickTax.Application.Interfaces;

namespace NickTax.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public AuthController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // GlobalExceptionHandler cuida se credenciais forem inválidas
        var response = await _usuarioService.AutenticarAsync(request);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var resultado = await _usuarioService.RefreshTokenAsync(request);
        return Ok(resultado);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _usuarioService.ForgotPasswordAsync(request);
        return Ok(new { mensagem = "Se o e-mail existir no sistema, um código de 6 dígitos foi enviado." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        await _usuarioService.ResetPasswordAsync(request);
        return Ok(new { mensagem = "Senha redefinida com sucesso!" });
    }
}