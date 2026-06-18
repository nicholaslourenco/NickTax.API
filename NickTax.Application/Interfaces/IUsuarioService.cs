using NickTax.Application.DTOs;

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