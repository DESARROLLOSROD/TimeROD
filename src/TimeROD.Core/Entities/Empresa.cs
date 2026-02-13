using System.Text.Json.Serialization;

namespace TimeROD.Core.Entities;

/// <summary>
/// Representa una empresa cliente del sistema (multi-tenant)
/// Una empresa puede tener múltiples proyectos, áreas y empleados
/// </summary>
public class Empresa : BaseEntity
{
    /// <summary>
    /// Nombre comercial de la empresa
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// RFC de la empresa (Registro Federal de Contribuyentes)
    /// </summary>
    public string RFC { get; set; } = string.Empty;

    /// <summary>
    /// Dirección fiscal de la empresa
    /// </summary>
    public string? Direccion { get; set; }

    /// <summary>
    /// Configuración personalizada en JSON
    /// Ejemplo: {"tolerancia_default": 10, "moneda": "MXN"}
    /// </summary>
    public string? ConfiguracionJson { get; set; }

    /// <summary>
    /// Indica si la empresa está activa o suspendida
    /// </summary>
    public bool Activa { get; set; } = true;

    // Navegación: una empresa tiene muchas áreas, proyectos, usuarios, etc.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ICollection<Area>? Areas { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ICollection<Usuario>? Usuarios { get; set; }
}
