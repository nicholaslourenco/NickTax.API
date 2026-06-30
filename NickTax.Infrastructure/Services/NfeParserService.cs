using System.Xml.Linq;
using System.Globalization;
using NickTax.Domain.Interfaces;
using NickTax.Domain.Models;

namespace NickTax.Infrastructure.Services
{
    public class NfeParserService : INfeParserService
    {
        public NotaFiscalDadosParseados ParseResumoNfe(string xmlConteudo)
        {
            try
            {
                var doc = XDocument.Parse(xmlConteudo);
                XNamespace ns = doc.Root?.Name.Namespace ?? XNamespace.None;

                // 1. O Elemento raiz agora é o docZip
                var raiz = doc.Root;
                if (raiz == null || (raiz.Name.LocalName != "docZip" && raiz.Name.LocalName != "resNFe"))
                {
                    throw new Exception("Estrutura XML inválida para identificação de NF-e.");
                }

                // 2. Tenta ler o atributo NSU da tag raiz (docZip)
                string nsuStr = raiz.Attribute("NSU")?.Value ?? "0";
                long nsu = long.Parse(nsuStr);

                // 3. Encontra a tag resNFe (pode ser a própria raiz ou uma filha)
                var resNFe = raiz.Name.LocalName == "resNFe" ? raiz : doc.Descendants(ns + "resNFe").FirstOrDefault();

                if (resNFe == null)
                {
                    throw new Exception("Estrutura 'resNFe' não encontrada no XML fornecido.");
                }

                // Extrai os valores internos do resumo da nota
                string chave = resNFe.Element(ns + "chNFe")?.Value ?? string.Empty;
                string cnpj = resNFe.Element(ns + "CNPJ")?.Value ?? string.Empty;
                string nome = resNFe.Element(ns + "xNome")?.Value ?? string.Empty;
                string dataStr = resNFe.Element(ns + "dhEmi")?.Value ?? string.Empty;
                string valorStr = resNFe.Element(ns + "vNF")?.Value ?? string.Empty;

                DateTime dataEmissao = DateTime.Parse(dataStr).ToUniversalTime();

                decimal valorTotal = decimal.Parse(valorStr, CultureInfo.InvariantCulture);

                return new NotaFiscalDadosParseados
                {
                    NSU = nsu,
                    ChaveAcesso = chave,
                    CnpjEmitente = cnpj,
                    NomeEmitente = nome,
                    DataEmissao = dataEmissao,
                    ValorTotal = valorTotal
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao realizar o parse do XML da NF-e: {ex.Message}", ex);
            }
        }
    }
}