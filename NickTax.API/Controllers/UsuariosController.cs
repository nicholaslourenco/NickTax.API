using Microsoft.AspNetCore.Mvc;
using NickTax.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using NickTax.Application.DTOs.Usuario;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Registrar([FromBody] CreateUsuarioRequest request)
    {
        var id = await _usuarioService.RegistrarUsuarioAsync(request);
        
        return Ok(new { id, mensagem = "Usuário criado com sucesso!" });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> ObterPerfil()
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(usuarioIdClaim))
            return Unauthorized();

        var usuarioId = Guid.Parse(usuarioIdClaim);
        var perfil = await _usuarioService.ObterPerfilAsync(usuarioId);

        return Ok(perfil);
    }
}