using System.ComponentModel.DataAnnotations;

namespace TimeROD.Core.DTOs;

public class RegistroEntradaDto
{
    [Required]
    public int EmpleadoId { get; set; }

    public string? Notas { get; set; }
}
