using NickTax.Domain.Interfaces;
using BCrypt.Net;

namespace NickTax.Infrastructure.Services
{
    public class SenhaService : ISenhaService
    {
        public string HashSenha(string senha)
        {
            // O BCrypt já gera o Salt internamente
            return BCrypt.Net.BCrypt.EnhancedHashPassword(senha);
        }

        public bool VerificarSenha(string senhaPura, string senhaHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(senhaPura, senhaHash);
        }
    }
}