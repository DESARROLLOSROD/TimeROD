namespace TimeROD.Core.Entities;

/// <summary>
/// Clase base para todas las entidades
/// Todas las tablas tendrán estos campos comunes
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// ID único de la entidad (llave primaria)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Fecha y hora cuando se creó el registro
    /// </summary>
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha y hora de la última actualización
    /// </summary>
    public DateTime? FechaActualizacion { get; set; }
}
