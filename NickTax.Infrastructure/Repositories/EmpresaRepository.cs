using Microsoft.EntityFrameworkCore;
using NickTax.Domain.Entities;
using NickTax.Domain.Interfaces;
using NickTax.Infrastructure.Context;

namespace NickTax.Infrastructure.Repositories
{
    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly AppDbContext _context;

        public EmpresaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmpresaEntity>> GetAllByUserAsync(Guid usuarioId)
        {
            return await _context.Empresas
                .Where(e => e.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<EmpresaEntity?> GetByIdAsync(int id, Guid usuarioId)
        {
            return await _context.Empresas
                .FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == usuarioId);
        }

        public async Task<bool> CnpjExistsAsync(string cnpj, Guid usuarioId)
        {
            return await _context.Empresas
                .AnyAsync(e => e.CNPJ == cnpj && e.UsuarioId == usuarioId);
        }

        public async Task AddAsync(EmpresaEntity empresa)
        {
            await _context.Empresas.AddAsync(empresa);
        }

        public void Update(EmpresaEntity empresa)
        {
            _context.Empresas.Update(empresa);
        }

        public void Delete(EmpresaEntity empresa)
        {
            _context.Empresas.Remove(empresa);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}