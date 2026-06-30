using NickTax.Application.DTOs.Login;
using NickTax.Application.DTOs.Token;
using NickTax.Application.DTOs.Usuario;

namespace NickTax.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<Guid> RegistrarUsuarioAsync(CreateUsuarioRequest request);

        Task<LoginResponse> AutenticarAsync(LoginRequest request);

        Task<UsuarioPerfilResponse> ObterPerfilAsync(Guid usuarioId);

        Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);

        Task ForgotPasswordAsync(ForgotPasswordRequest request);

        Task ResetPasswordAsync(ResetPasswordRequest request);
    }
}