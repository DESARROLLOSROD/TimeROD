using System.ComponentModel.DataAnnotations;

namespace TimeROD.Core.Entities;

public class Horario
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public TimeSpan HoraEntrada { get; set; }

    [Required]
    public TimeSpan HoraSalida { get; set; }

    [Range(0, 120)]
    public int ToleranciaMinutos { get; set; } = 0;

    public bool Activo { get; set; } = true;
    
    // Navigation properties
    public ICollection<Area> Areas { get; set; } = new List<Area>();
    public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
