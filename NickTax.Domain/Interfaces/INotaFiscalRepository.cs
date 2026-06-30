using NickTax.Domain.Entities;

namespace NickTax.Domain.Interfaces
{
    public interface INotaFiscalRepository
    {
        Task<IEnumerable<NotaFiscalEntity>> GetAllByEmpresaAsync(int empresaId, Guid usuarioId);
        Task<NotaFiscalEntity?> GetByIdAsync(int id, Guid usuarioId);
        Task<bool> ChaveAcessoExistsAsync(string chaveAcesso);
        Task AddAsync(NotaFiscalEntity notaFiscal);
        void Delete(NotaFiscalEntity notaFiscal);
        Task SaveChangesAsync();
    }
}