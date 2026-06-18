namespace NickTax.Domain.Interfaces
{
    public interface ISenhaService
    {
        string HashSenha(string senha);
        bool VerificarSenha(string senhaPura, string senhaHash);
    }
}