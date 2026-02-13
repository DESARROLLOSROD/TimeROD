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
    public Empresa Empresa { get; set; } = null!;
    public Usuario? Supervisor { get; set; }
    public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
