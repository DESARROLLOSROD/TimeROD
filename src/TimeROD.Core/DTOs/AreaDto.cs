namespace TimeROD.Core.DTOs;

public class AreaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activa { get; set; }
    public int EmpresaId { get; set; }
    public string? EmpresaNombre { get; set; }
    
    public int? SupervisorId { get; set; }
    public string? SupervisorNombre { get; set; }
}
