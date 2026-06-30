using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NickTax.Application.DTOs.Empresa;
using NickTax.Application.DTOs.NotaFiscal;
using NickTax.Application.Interfaces;

namespace NickTax.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmpresasController : ControllerBase
    {
        private readonly IEmpresaService _empresaService;
        private readonly INotaFiscalService _notaFiscalService;

        public EmpresasController(IEmpresaService empresaService, INotaFiscalService notaFiscalService)
        {
            _empresaService = empresaService;
            _notaFiscalService = notaFiscalService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarioId = ObterUsuarioIdLogado();
            var empresas = await _empresaService.GetAllByUserAsync(usuarioId);
            return Ok(empresas);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuarioId = ObterUsuarioIdLogado();
            var empresa = await _empresaService.GetByIdAsync(id, usuarioId);

            if (empresa == null)
                return NotFound(new { mensagem = "Empresa não encontrada ou acesso negado." });

            return Ok(empresa);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmpresaDTO dto)
        {
            var usuarioId = ObterUsuarioIdLogado();
            try
            {
                var novaEmpresa = await _empresaService.CreateAsync(dto, usuarioId);
                return CreatedAtAction(nameof(GetById), new { id = novaEmpresa.Id }, novaEmpresa);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmpresaDTO dto)
        {
            var usuarioId = ObterUsuarioIdLogado();
            try
            {
                var atualizado = await _empresaService.UpdateAsync(id, dto, usuarioId);
                if (!atualizado) return NotFound(new { mensagem = "Empresa não encontrada ou acesso negado." });
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioId = ObterUsuarioIdLogado();
            var deletado = await _empresaService.DeleteAsync(id, usuarioId);
            if (!deletado) return NotFound(new { mensagem = "Empresa não encontrada ou acesso negado." });
            return NoContent();
        }

        [HttpPost("{empresaId}/notas/sincronizar")]
        public async Task<ActionResult<SincronizacaoResultadoDTO>> SincronizarNotas(int empresaId)
        {
            try
            {
                var usuarioId = ObterUsuarioIdLogado();
                var resultado = await _notaFiscalService.SincronizarNotasSefazAsync(empresaId, usuarioId);
                return Ok(resultado);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                var mensagemReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Erro interno ao processar sincronização: {mensagemReal}");
            }
        }

        private Guid ObterUsuarioIdLogado()
        {
            var nameIdentifierClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(nameIdentifierClaim) || !Guid.TryParse(nameIdentifierClaim, out var usuarioId))
            {
                throw new UnauthorizedAccessException("Usuário inválido ou token expirado.");
            }
            return usuarioId;
        }
    }
}