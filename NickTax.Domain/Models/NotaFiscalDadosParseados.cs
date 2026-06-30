namespace NickTax.Domain.Models
{
    public class NotaFiscalDadosParseados
    {
        public long NSU { get; set; }
        public string ChaveAcesso { get; set; } = string.Empty;
        public string CnpjEmitente { get; set; } = string.Empty;
        public string NomeEmitente { get; set; } = string.Empty;
        public DateTime DataEmissao { get; set; }
        public decimal ValorTotal { get; set; }
    }
}