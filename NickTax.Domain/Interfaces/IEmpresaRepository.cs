using NickTax.Domain.Entities;

namespace NickTax.Domain.Interfaces
{
    public interface IEmpresaRepository
    {
        Task<IEnumerable<EmpresaEntity>> GetAllByUserAsync(Guid usuarioId);
        Task<EmpresaEntity?> GetByIdAsync(int id, Guid usuarioId);
        Task<bool> CnpjExistsAsync(string cnpj, Guid usuarioId);
        Task AddAsync(EmpresaEntity empresa);
        void Update(EmpresaEntity empresa);
        void Delete(EmpresaEntity empresa);
        Task SaveChangesAsync();
    }
}