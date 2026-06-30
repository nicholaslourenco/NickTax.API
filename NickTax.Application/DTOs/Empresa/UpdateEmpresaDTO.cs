namespace NickTax.Application.DTOs.Empresa
{
    public class UpdateEmpresaDTO
    {
        public string RazaoSocial { get; set; } = string.Empty;
        public byte[]? CertificadoDigital { get; set; }
        public string? CertificadoSenha { get; set; } = string.Empty;
    }
}