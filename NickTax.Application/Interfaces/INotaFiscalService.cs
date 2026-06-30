using NickTax.Application.DTOs;
using NickTax.Application.DTOs.NotaFiscal;

namespace NickTax.Application.Interfaces
{
    public interface INotaFiscalService
    {
        Task<IEnumerable<NotaFiscalDTO>> GetAllByEmpresaAsync(int empresaId, Guid usuarioId);
        Task<NotaFiscalDTO?> GetByIdAsync(int id, Guid usuarioId);
        Task<NotaFiscalDTO> CreateAsync(int empresaId, CreateNotaFiscalDTO dto, Guid usuarioId);
        Task<bool> DeleteAsync(int id, Guid usuarioId);
        Task<SincronizacaoResultadoDTO> SincronizarNotasSefazAsync(int empresaId, Guid usuarioId);
    }
}