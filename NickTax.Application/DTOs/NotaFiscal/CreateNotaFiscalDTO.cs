namespace NickTax.Application.DTOs.NotaFiscal
{
    public class CreateNotaFiscalDTO
    {
        public long NSU { get; set; }
        public string ChaveAcesso { get; set; } = string.Empty;
        public string ConteudoXML { get; set; } = string.Empty;
        public DateTime DataEmissao { get; set; }
        public decimal ValorTotal { get; set; }
    }
}