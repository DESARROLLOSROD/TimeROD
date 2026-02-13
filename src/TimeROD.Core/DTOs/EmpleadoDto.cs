namespace TimeROD.Core.DTOs;

public class EmpleadoDto
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public string? EmpresaNombre { get; set; }
    public int AreaId { get; set; }
    public string? AreaNombre { get; set; }
    public int? UsuarioId { get; set; }
    public string? UsuarioNombre { get; set; }
    public string NumeroEmpleado { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; }
    public decimal SalarioDiario { get; set; }
    public int? TurnoId { get; set; }
    public string? IdBiometrico { get; set; }
    public bool Activo { get; set; }
    public string? Puesto { get; set; }
}
