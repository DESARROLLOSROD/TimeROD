using System.ComponentModel.DataAnnotations;

namespace TimeROD.Core.DTOs;

public class CreateEmpresaDto
{
    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^[A-Z&Ñ]{3,4}\d{6}[A-Z\d]{3}$", ErrorMessage = "El RFC no tiene un formato válido")]
    public string RFC { get; set; } = string.Empty;

    public string? Direccion { get; set; }
    public string? ConfiguracionJson { get; set; }
}
