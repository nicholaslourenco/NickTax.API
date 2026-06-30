using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using NickTax.Domain.Interfaces;
using NickTax.Application.Interfaces;
using NickTax.Domain.Entities;
using NickTax.Domain.Exceptions;
using NickTax.Application.DTOs.Usuario;
using NickTax.Application.DTOs.Token;
using NickTax.Application.DTOs.Login;

namespace NickTax.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISenhaService _senhaService;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(IUsuarioRepository usuarioRepository, ISenhaService senhaService, ITokenService tokenService, IMapper mapper, ILogger<UsuarioService> logger, IEmailService emailService)
        {
            _usuarioRepository = usuarioRepository;
            _senhaService = senhaService;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<Guid> RegistrarUsuarioAsync(CreateUsuarioRequest request)
        {
            // Regra de Negócio: Email Único
            var existente = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (existente != null)
                throw new DomainException(Messages.Usuario.EmailDuplicado);

            // Mapeamento e Criptografia
            var novoUsuario = new UsuarioEntity
            {
                Nome = request.Nome,
                Email = request.Email,
                SenhaHash = _senhaService.HashSenha(request.Senha)
            };

            var resultado = await _usuarioRepository.AddAsync(novoUsuario);

            return resultado.Id;
        }

        public async Task<LoginResponse> AutenticarAsync(LoginRequest request)
        {
            // Buscar o usuário pelo e-mail
            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);

            if (usuario == null || !_senhaService.VerificarSenha(request.Senha, usuario.SenhaHash))
            {
                throw new DomainException(Messages.Usuario.ErroLogin);
            }

            var token = _tokenService.GerarToken(usuario);

            return new LoginResponse(usuario.Email, token, "Login realizado com sucesso!");
        }

        public async Task<UsuarioPerfilResponse> ObterPerfilAsync(Guid usuarioId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);

            if (usuario == null)
                throw new DomainException("Usuário não encontrado.");

            return _mapper.Map<UsuarioPerfilResponse>(usuario);
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var usuarioIdStr = _tokenService.ExtrairUsuarioIdDeTokenExpirado(request.Token);
            if (string.IsNullOrEmpty(usuarioIdStr))
                throw new DomainException("Token inválido.");


            var usuario = await _usuarioRepository.GetByIdAsync(Guid.Parse(usuarioIdStr));

            if (usuario == null ||
                usuario.RefreshToken != request.RefreshToken ||
                usuario.RefreshTokenExpiration <= DateTime.UtcNow)
            {
                throw new DomainException("Refresh token inválido ou expirado.");
            }

            var novoJwt = _tokenService.GerarToken(usuario);
            var novoRefreshToken = _tokenService.GerarRefreshTokenValue();

            usuario.UpdateRefreshToken(novoRefreshToken, DateTime.UtcNow.AddDays(7));
            await _usuarioRepository.UpdateAsync(usuario);

            _logger.LogInformation("Token renovado com sucesso para o usuário ID: {UsuarioId}", usuarioIdStr);

            return new RefreshTokenResponse(novoJwt, novoRefreshToken);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);

            if (usuario == null)
            {
                throw new DomainException("Usuário não encontrado com este e-mail.");
            }

            var codigoAleatorio = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

            var expiracao = DateTime.UtcNow.AddMinutes(15);

            usuario.GeneratePasswordResetCode(codigoAleatorio, expiracao);
            await _usuarioRepository.UpdateAsync(usuario);

            await _emailService.EnviarCodigoRecuperacaoAsync(usuario.Email, codigoAleatorio);

            _logger.LogInformation("Código de recuperação gerado para o e-mail: {Email}", request.Email);
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest request)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);

            if (usuario == null)
                throw new DomainException("Solicitação inválida.");

            if (usuario.PasswordResetCode == null ||
                usuario.PasswordResetCode != request.Code ||
                usuario.ResetCodeExpiration <= DateTime.UtcNow)
            {
                throw new DomainException("Código de verificação inválido ou expirado.");
            }

            var novaSenhaHash = _senhaService.HashSenha(request.NewPassword);

            usuario.UpdateSenha(novaSenhaHash);
            usuario.ClearPasswordResetCode();

            await _usuarioRepository.UpdateAsync(usuario);

            _logger.LogInformation("Senha alterada com sucesso para o usuário: {Email}", request.Email);
        }
    }
}