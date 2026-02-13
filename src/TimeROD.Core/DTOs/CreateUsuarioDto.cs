using System.ComponentModel.DataAnnotations;

namespace TimeROD.Core.DTOs;

public class CreateUsuarioDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required]
    public string Rol { get; set; } = string.Empty; // "Admin", "Supervisor", "Empleado"

    [Required]
    public int EmpresaId { get; set; }
}
