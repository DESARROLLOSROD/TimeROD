using System.Text.Json.Serialization;

namespace TimeROD.Core.Entities;

/// <summary>
/// Registro de asistencia de un empleado
/// </summary>
public class Asistencia : BaseEntity
{
    /// <summary>
    /// ID del empleado que registra la asistencia
    /// </summary>
    public int EmpleadoId { get; set; }

    /// <summary>
    /// Fecha del registro (se guarda con hora 00:00:00 para compatibilidad)
    /// </summary>
    public DateTime Fecha { get; set; }

    /// <summary>
    /// Hora de entrada registrada
    /// </summary>
    public DateTime? HoraEntrada { get; set; }

    /// <summary>
    /// Hora de salida registrada
    /// </summary>
    public DateTime? HoraSalida { get; set; }

    /// <summary>
    /// Tipo de registro de asistencia
    /// </summary>
    public TipoAsistencia Tipo { get; set; } = TipoAsistencia.Normal;

    /// <summary>
    /// Notas o comentarios sobre la asistencia
    /// </summary>
    public string? Notas { get; set; }

    /// <summary>
    /// Indica si el registro fue aprobado por un supervisor
    /// </summary>
    public bool Aprobado { get; set; } = true;

    /// <summary>
    /// Horas totales trabajadas (calculadas)
    /// </summary>
    public decimal? HorasTrabajadas { get; set; }

    /// <summary>
    /// Indica si llegó tarde
    /// </summary>
    public bool LlegadaTardia { get; set; } = false;

    /// <summary>
    /// Minutos de retraso
    /// </summary>
    public int? MinutosRetraso { get; set; }

    // Navegación
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Empleado? Empleado { get; set; }
}

/// <summary>
/// Tipos de registro de asistencia
/// </summary>
public enum TipoAsistencia
{
    /// <summary>
    /// Asistencia normal (día de trabajo regular)
    /// </summary>
    Normal = 1,

    /// <summary>
    /// Tiempo extra / Horas extras
    /// </summary>
    TiempoExtra = 2,

    /// <summary>
    /// Incidencia (falta justificada, permiso, etc.)
    /// </summary>
    Incidencia = 3,

    /// <summary>
    /// Falta injustificada
    /// </summary>
    Falta = 4,

    /// <summary>
    /// Día festivo trabajado
    /// </summary>
    DiaFestivo = 5,

    /// <summary>
    /// Descanso trabajado
    /// </summary>
    DescansoTrabajado = 6
}
