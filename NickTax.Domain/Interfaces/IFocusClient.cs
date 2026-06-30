using NickTax.Domain.Models;

namespace NickTax.Domain.Interfaces
{
    public interface IFocusClient
    {
        Task<bool> CadastrarEmpresaAsync(string cnpj, string razaoSocial, byte[] certificadoBytes, string senha);

        Task<bool> DispararPesquisaNotasAsync(string cnpj);

        Task<FocusQueryResult> ConsultarNotasAsync(string cnpj, long ultimoNSU);
    }
}