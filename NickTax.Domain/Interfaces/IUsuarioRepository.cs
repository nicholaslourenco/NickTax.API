using NickTax.Domain.Entities;

namespace NickTax.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<UsuarioEntity> AddAsync(UsuarioEntity usuario);
        Task<UsuarioEntity?> GetByEmailAsync(string email);
        Task<UsuarioEntity?> GetByIdAsync(Guid id);
        Task UpdateAsync(UsuarioEntity usuario);
    }
}