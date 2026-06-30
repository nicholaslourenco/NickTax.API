namespace NickTax.Application.DTOs.Login;

public record LoginResponse(string Email, string Token, string Mensagem);