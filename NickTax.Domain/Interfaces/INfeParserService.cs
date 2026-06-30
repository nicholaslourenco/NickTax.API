using NickTax.Domain.Models; // Ou onde quer que você monte o objeto de transição

namespace NickTax.Domain.Interfaces
{
    public interface INfeParserService
    {
        NotaFiscalDadosParseados ParseResumoNfe(string xmlConteudo);
    }
}