using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NickTax.Domain.Entities
{
    [Table("NotasFiscais")]
    public class NotaFiscalEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmpresaId { get; set; }

        [Required]
        public long NSU { get; set; }

        [Required, MaxLength(44)]
        public string ChaveAcesso { get; set; } = string.Empty;

        [Required, MaxLength(14)]
        public string CnpjEmitente { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string NomeEmitente { get; set; } = string.Empty;

        [Required]
        public string ConteudoXML { get; set; } = string.Empty;

        [Required]
        public DateTime DataEmissao { get; set; }

        [Required]
        [Column(TypeName = "numeric(10,2)")]
        public decimal ValorTotal { get; set; }

        [ForeignKey("EmpresaId")]
        public virtual EmpresaEntity Empresa { get; set; } = null!;
    }
}