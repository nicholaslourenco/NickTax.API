using NickTax.Application.DTOs.Empresa;

namespace NickTax.Application.Interfaces
{
    public interface IEmpresaService
    {
        Task<IEnumerable<EmpresaDTO>> GetAllByUserAsync(Guid usuarioId);
        Task<EmpresaDTO?> GetByIdAsync(int id, Guid usuarioId);
        Task<EmpresaDTO> CreateAsync(CreateEmpresaDTO dto, Guid usuarioId);
        Task<bool> UpdateAsync(int id, UpdateEmpresaDTO dto, Guid usuarioId);
        Task<bool> DeleteAsync(int id, Guid usuarioId);
    }
}