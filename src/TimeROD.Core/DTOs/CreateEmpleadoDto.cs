using System.ComponentModel.DataAnnotations;

namespace TimeROD.Core.DTOs;

public class CreateEmpleadoDto
{
    [Required]
    public int EmpresaId { get; set; }

    [Required]
    public int AreaId { get; set; }

    public int? UsuarioId { get; set; }

    [Required]
    public string NumeroEmpleado { get; set; } = string.Empty;

    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string Apellidos { get; set; } = string.Empty;

    [Required]
    public DateTime FechaIngreso { get; set; }

    [Required]
    public decimal SalarioDiario { get; set; }

    public int? TurnoId { get; set; }

    public string? IdBiometrico { get; set; }

    public string? Puesto { get; set; }
}
