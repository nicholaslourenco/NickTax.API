namespace NickTax.Domain.Interfaces
{
    public interface ICertificadoService
    {
        bool ValidarCertificado(byte[] certificadoBytes, string senha, string cnpjEmpresa, out string resultadoMensagem);
        DateTime ObterDataValidade(byte[] certificadoBytes, string senha);
    }
}