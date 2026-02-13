using System.Text.Json.Serialization;

namespace TimeROD.Core.Entities;

/// <summary>
/// Área o Departamento dentro de una empresa
/// Ejemplo: Producción, Mantenimiento, Administración
/// </summary>
public class Area : BaseEntity
{
    /// <summary>
    /// ID de la empresa a la que pertenece
    /// </summary>
    public int EmpresaId { get; set; }

    /// <summary>
    /// Nombre del área o departamento
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// ID del supervisor responsable del área (opcional)
    /// </summary>
    public int? SupervisorId { get; set; }

    /// <summary>
    /// Descripción o notas del área
    /// </summary>
    public string? Descripcion { get; set; }

    /// <summary>
    /// Indica si el área está activa
    /// </summary>
    public bool Activa { get; set; } = true;

    // Navegación
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Empresa? Empresa { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Usuario? Supervisor { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ICollection<Empleado>? Empleados { get; set; }
}
