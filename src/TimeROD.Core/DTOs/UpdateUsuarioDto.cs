using System.ComponentModel.DataAnnotations;

namespace TimeROD.Core.DTOs;

public class UpdateUsuarioDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Password { get; set; } // Opcional, solo si se quiere cambiar

    [Required]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required]
    public string Rol { get; set; } = string.Empty;

    public bool Activo { get; set; }

    [Required]
    public int EmpresaId { get; set; }
}
