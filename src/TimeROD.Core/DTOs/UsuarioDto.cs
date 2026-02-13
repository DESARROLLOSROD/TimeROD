namespace TimeROD.Core.DTOs;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public int EmpresaId { get; set; }
    public string? EmpresaNombre { get; set; }
    public DateTime? UltimoAcceso { get; set; }
}
