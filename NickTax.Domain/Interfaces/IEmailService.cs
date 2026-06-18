namespace NickTax.Domain.Interfaces;

public interface IEmailService
{
    Task EnviarCodigoRecuperacaoAsync(string emailDestinatario, string codigo);
}