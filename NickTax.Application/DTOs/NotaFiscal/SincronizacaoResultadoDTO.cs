namespace NickTax.Application.DTOs.NotaFiscal
{
    public class SincronizacaoResultadoDTO
    {
        public string CStat { get; set; } = string.Empty;
        public string XMotivo { get; set; } = string.Empty;
        public int QuantidadeNovasNotas { get; set; }
        public long NovoUltimoNSU { get; set; }
    }
}