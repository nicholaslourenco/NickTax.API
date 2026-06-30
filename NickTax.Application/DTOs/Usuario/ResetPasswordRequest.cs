namespace NickTax.Application.DTOs.Usuario;

public record ResetPasswordRequest(string Email, string Code, string NewPassword);