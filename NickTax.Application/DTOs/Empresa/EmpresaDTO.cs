namespace NickTax.Application.DTOs.Empresa
{
    public class EmpresaDTO
    {
        public int Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string CNPJ { get; set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;
        public byte[] CertificadoDigital { get; set; } = Array.Empty<byte>();
        public long UltimoNSU { get; set; }
    }
}