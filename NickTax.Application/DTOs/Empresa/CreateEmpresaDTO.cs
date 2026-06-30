using System.ComponentModel.DataAnnotations;

namespace NickTax.Application.DTOs.Empresa
{
    public class CreateEmpresaDTO
    {
        [Required(ErrorMessage = "O CNPJ é obrigatório.")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "O CNPJ deve conter exatamente 14 dígitos.")]
        public string CNPJ { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Razão Social é obrigatória.")]
        [MinLength(3, ErrorMessage = "A Razão Social deve ter pelo menos 3 caracteres.")]
        public string RazaoSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "O arquivo do certificado digital é obrigatório.")]
        public byte[] CertificadoDigital { get; set; } = null!;

        [Required(ErrorMessage = "A senha do certificado digital é obrigatória.")]
        [MinLength(1, ErrorMessage = "A senha do certificado não pode estar vazia.")]
        public string CertificadoSenha { get; set; } = string.Empty;
    }
}