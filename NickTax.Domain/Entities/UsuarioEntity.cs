using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NickTax.Domain.Entities
{
    [Table("Usuarios")]
    public class UsuarioEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string SenhaHash { get; set; } = string.Empty;

        public virtual ICollection<EmpresaEntity> Empresas { get; set; } = new List<EmpresaEntity>();

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiration { get; set; }

        public string? PasswordResetCode { get; private set; }

        public DateTime? ResetCodeExpiration { get; private set; }

        public void UpdateRefreshToken(string token, DateTime expiration)
        {
            RefreshToken = token;
            RefreshTokenExpiration = expiration;
        }

        public void UpdateSenha(string novaSenhaHash)
        {
            SenhaHash = novaSenhaHash;
        }

        public void GeneratePasswordResetCode(string code, DateTime expiration)
        {
            PasswordResetCode = code;
            ResetCodeExpiration = expiration;
        }

        public void ClearPasswordResetCode()
        {
            PasswordResetCode = null;
            ResetCodeExpiration = null;
        }
    }
}