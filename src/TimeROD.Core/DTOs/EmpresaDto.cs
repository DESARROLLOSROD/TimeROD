using System.Text.Json.Serialization;

namespace TimeROD.Core.DTOs;

public class EmpresaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string RFC { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    
    // Devolvemos el JSON tal cual o podríamos parsearlo si es necesario
    public string? ConfiguracionJson { get; set; }
    
    public bool Activa { get; set; }
}
