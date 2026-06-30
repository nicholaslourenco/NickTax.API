using Microsoft.EntityFrameworkCore;
using NickTax.Domain.Entities;
using NickTax.Domain.Interfaces;
using NickTax.Infrastructure.Context;

namespace NickTax.Infrastructure.Repositories
{
    public class NotaFiscalRepository : INotaFiscalRepository
    {
        private readonly AppDbContext _context;

        public NotaFiscalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NotaFiscalEntity>> GetAllByEmpresaAsync(int empresaId, Guid usuarioId)
        {
            // Valida se nota pertence à empresa e se empresa pertence ao usuário
            return await _context.NotasFiscais
                .Include(nf => nf.Empresa)
                .Where(nf => nf.EmpresaId == empresaId && nf.Empresa.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<NotaFiscalEntity?> GetByIdAsync(int id, Guid usuarioId)
        {
            return await _context.NotasFiscais
                .Include(nf => nf.Empresa)
                .FirstOrDefaultAsync(nf => nf.Id == id && nf.Empresa.UsuarioId == usuarioId);
        }

        public async Task<bool> ChaveAcessoExistsAsync(string chaveAcesso)
        {
            return await _context.NotasFiscais.AnyAsync(nf => nf.ChaveAcesso == chaveAcesso);
        }

        public async Task AddAsync(NotaFiscalEntity notaFiscal)
        {
            await _context.NotasFiscais.AddAsync(notaFiscal);
        }

        public void Delete(NotaFiscalEntity notaFiscal)
        {
            _context.NotasFiscais.Remove(notaFiscal);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}