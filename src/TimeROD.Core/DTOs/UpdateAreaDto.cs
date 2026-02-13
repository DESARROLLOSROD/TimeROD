using System.ComponentModel.DataAnnotations;

namespace TimeROD.Core.DTOs;

public class UpdateAreaDto
{
    [Required]
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }
    
    public int? SupervisorId { get; set; }
    
    public bool Activa { get; set; }
    
    [Required]
    public int EmpresaId { get; set; }
}
