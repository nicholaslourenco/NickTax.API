using Microsoft.EntityFrameworkCore;
using NickTax.Domain.Entities;
using NickTax.Domain.Interfaces;
using NickTax.Infrastructure.Context;

namespace NickTax.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task UpdateAsync(UsuarioEntity usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<UsuarioEntity> AddAsync(UsuarioEntity usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<UsuarioEntity?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UsuarioEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Usuarios.FindAsync(id);
        }
    }
}