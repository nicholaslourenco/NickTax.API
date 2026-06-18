using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NickTax.Domain.Entities
{
    [Table("Empresas")]
    public class EmpresaEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid UsuarioId { get; set; }

        [Required, MaxLength(14)]
        public string CNPJ { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string RazaoSocial { get; set; } = string.Empty;

        [Required]
        public byte[] CertificadoDigital { get; set; } = Array.Empty<byte>();

        [Required]
        public string CertificadoSenha { get; set; } = string.Empty;

        [Required]
        public long UltimoNSU { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual UsuarioEntity Usuario { get; set; } = null!;
        public virtual ICollection<NotaFiscalEntity> NotasFiscais { get; set; } = new List<NotaFiscalEntity>();
    }
}