namespace TimeROD.Core.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UsuarioDto Usuario { get; set; } = new();
}
