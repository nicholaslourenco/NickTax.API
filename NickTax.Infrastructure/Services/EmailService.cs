using Microsoft.Extensions.Logging;
using NickTax.Domain.Interfaces;

namespace NickTax.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task EnviarCodigoRecuperacaoAsync(string emailDestinatario, string codigo)
    {
        // Simulando o envio de e-mail gravando no log do sistema
        _logger.LogInformation("==================================================");
        _logger.LogInformation("[ENVIO DE E-MAIL SIMULADO]");
        _logger.LogInformation("Para: {Email}", emailDestinatario);
        _logger.LogInformation("Assunto: NickTax - Código de Recuperação de Senha");
        _logger.LogInformation("Mensagem: Seu código de verificação é: {Codigo}. Ele expira em 15 minutos.", codigo);
        _logger.LogInformation("==================================================");

        return Task.CompletedTask;
    }
}