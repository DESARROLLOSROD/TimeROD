using System.ComponentModel.DataAnnotations;

namespace TimeROD.Core.DTOs;

public class RegistroSalidaDto
{
    [Required]
    public int EmpleadoId { get; set; }

    public string? Notas { get; set; }
}
