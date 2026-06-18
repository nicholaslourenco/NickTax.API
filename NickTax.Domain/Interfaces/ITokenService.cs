using NickTax.Domain.Entities;

namespace NickTax.Domain.Interfaces;

public interface ITokenService
{
    string GerarToken(UsuarioEntity usuario);
    string GerarRefreshTokenValue();
    string? ExtrairUsuarioIdDeTokenExpirado(string token);
}