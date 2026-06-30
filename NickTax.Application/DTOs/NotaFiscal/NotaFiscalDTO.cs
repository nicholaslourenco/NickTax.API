namespace NickTax.Application.DTOs.NotaFiscal
{
    public class NotaFiscalDTO
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public long NSU { get; set; }
        public string ChaveAcesso { get; set; } = string.Empty;
        public string ConteudoXML { get; set; } = string.Empty;
        public DateTime DataEmissao { get; set; }
        public decimal ValorTotal { get; set; }
    }
}