using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using NickTax.Domain.Interfaces;

namespace NickTax.Infrastructure.Services
{
    public class CertificadoService : ICertificadoService
    {
        public bool ValidarCertificado(byte[] certificadoBytes, string senha, string cnpjEmpresa, out string resultadoMensagem)
        {
            try
            {
                // PADRÃO MODERNO (.NET 9/10): X509CertificateLoader
                using var certificado = X509CertificateLoader.LoadPkcs12(
                    certificadoBytes,
                    senha,
                    X509KeyStorageFlags.EphemeralKeySet);

                // 1. Validação de Data de Validade
                if (DateTime.Now > certificado.NotAfter)
                {
                    resultadoMensagem = $"Certificado expirado em: {certificado.NotAfter}";
                    return false;
                }

                if (DateTime.Now < certificado.NotBefore)
                {
                    resultadoMensagem = $"Certificado ainda não é válido (Início: {certificado.NotBefore})";
                    return false;
                }

                // 2. Extração e Validação do CNPJ dentro do Certificado
                string subject = certificado.Subject;
                string cnpjEncontrado = ExtrairCnpjDoSubject(subject);

                string cnpjEmpresaLimpo = Regex.Replace(cnpjEmpresa, @"[^\d]", "");

                if (string.IsNullOrEmpty(cnpjEncontrado) || cnpjEncontrado != cnpjEmpresaLimpo)
                {
                    resultadoMensagem = $"CNPJ do certificado ({cnpjEncontrado}) não confere com o da Empresa ({cnpjEmpresaLimpo}).";
                    return false;
                }

                resultadoMensagem = "Certificado válido e pronto para uso!";
                return true;
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                resultadoMensagem = "Senha incorreta ou arquivo de certificado corrompido.";
                return false;
            }
            catch (Exception ex)
            {
                resultadoMensagem = $"Erro inesperado ao ler certificado: {ex.Message}";
                return false;
            }
        }

        public DateTime ObterDataValidade(byte[] certificadoBytes, string senha)
        {
            // PADRÃO MODERNO (.NET 9/10): X509CertificateLoader
            using var certificado = X509CertificateLoader.LoadPkcs12(
                certificadoBytes,
                senha,
                X509KeyStorageFlags.EphemeralKeySet);

            return certificado.NotAfter;
        }

        private string ExtrairCnpjDoSubject(string subject)
        {
            // Procura por dois pontos (:) seguido de 14 dígitos,
            // garante que depois desses 14 dígitos não haja mais números
            var match = Regex.Match(subject, @":(\d{14})\b");

            if (match.Success)
            {
                // Retorna apenas o grupo 1, ou seja, o que está dentro dos parênteses (os 14 números)
                return match.Groups[1].Value;
            }

            // Caso certificadora use padrão diferente sem dois pontos, tenta pegar 14 dígitos isolados que não façam parte de número maior
            var matchIsolado = Regex.Match(subject, @"\b\d{14}\b");
            return matchIsolado.Success ? matchIsolado.Value : string.Empty;
        }
    }
}