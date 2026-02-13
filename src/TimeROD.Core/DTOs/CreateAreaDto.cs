using System.ComponentModel.DataAnnotations;

namespace TimeROD.Core.DTOs;

public class CreateAreaDto
{
    [Required]
    public int EmpresaId { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }
    
    public int? SupervisorId { get; set; }
}
