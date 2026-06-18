namespace NickTax.Application.DTOs;

public record LoginResponse(string Email, string Token, string Mensagem);