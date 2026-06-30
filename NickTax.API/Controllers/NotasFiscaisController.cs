using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NickTax.Application.DTOs;
using NickTax.Application.DTOs.NotaFiscal;
using NickTax.Application.Interfaces;
using System.Security.Claims;

namespace NickTax.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api")]
    public class NotasFiscaisController : ControllerBase
    {
        private readonly INotaFiscalService _notaFiscalService;

        public NotasFiscaisController(INotaFiscalService notaFiscalService)
        {
            _notaFiscalService = notaFiscalService;
        }

        [HttpGet("empresas/{empresaId:int}/notasfiscais")]
        public async Task<IActionResult> GetAllByEmpresa(int empresaId)
        {
            try
            {
                var usuarioId = ObterUsuarioIdLogado();
                var notas = await _notaFiscalService.GetAllByEmpresaAsync(empresaId, usuarioId);
                return Ok(notas);
            }
            catch (UnauthorizedAccessException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        [HttpGet("notasfiscais/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuarioId = ObterUsuarioIdLogado();
            var nota = await _notaFiscalService.GetByIdAsync(id, usuarioId);

            if (nota == null)
                return NotFound(new { mensagem = "Nota fiscal não encontrada ou você não tem permissão." });

            return Ok(nota);
        }

        [HttpPost("empresas/{empresaId:int}/notasfiscais")]
        public async Task<IActionResult> Create(int empresaId, [FromBody] CreateNotaFiscalDTO dto)
        {
            var usuarioId = ObterUsuarioIdLogado();

            try
            {
                var novaNota = await _notaFiscalService.CreateAsync(empresaId, dto, usuarioId);
                return CreatedAtAction(nameof(GetById), new { id = novaNota.Id }, novaNota);
            }
            catch (UnauthorizedAccessException)
            {
                return NotFound(new { mensagem = "Empresa não encontrada ou você não tem permissão." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpDelete("notasfiscais/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioId = ObterUsuarioIdLogado();
            var deletado = await _notaFiscalService.DeleteAsync(id, usuarioId);

            if (!deletado)
                return NotFound(new { message = "Nota fiscal não encontrada ou você não tem permissão." });

            return NoContent();
        }

        private Guid ObterUsuarioIdLogado()
        {
            var nameIdentifierClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(nameIdentifierClaim) || !Guid.TryParse(nameIdentifierClaim, out var usuarioId))
            {
                throw new UnauthorizedAccessException("Usuário inválido ou não identificado no token.");
            }

            return usuarioId;
        }
    }
}