using System.Text.Json.Serialization;

namespace TimeROD.Core.Entities;

/// <summary>
/// Empleado de la empresa
/// Un empleado puede o no tener usuario en el sistema
/// </summary>
public class Empleado : BaseEntity
{
    /// <summary>
    /// ID de la empresa
    /// </summary>
    public int EmpresaId { get; set; }

    /// <summary>
    /// ID del área a la que pertenece
    /// </summary>
    public int AreaId { get; set; }

    /// <summary>
    /// ID del usuario (si el empleado tiene acceso al sistema)
    /// </summary>
    public int? UsuarioId { get; set; }

    /// <summary>
    /// Número de empleado único en la empresa
    /// </summary>
    public string NumeroEmpleado { get; set; } = string.Empty;

    /// <summary>
    /// Nombre(s) del empleado
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Apellidos del empleado
    /// </summary>
    public string Apellidos { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de ingreso a la empresa
    /// </summary>
    public DateTime FechaIngreso { get; set; }

    /// <summary>
    /// Salario diario del empleado (para cálculos de nómina)
    /// </summary>
    public decimal SalarioDiario { get; set; }

    /// <summary>
    /// ID del turno asignado al empleado (opcional)
    /// </summary>
    public int? TurnoId { get; set; }

    /// <summary>
    /// ID en el sistema biométrico (para asociar marcas)
    /// </summary>
    public string? IdBiometrico { get; set; }

    /// <summary>
    /// Indica si el empleado está activo
    /// </summary>
    public bool Activo { get; set; } = true;

    /// <summary>
    /// ID del horario específico del empleado (opcional, sobreescribe el del área)
    /// </summary>
    public int? HorarioId { get; set; }

    // Navegación
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Horario? Horario { get; set; }

    /// <summary>
    /// Puesto o cargo del empleado
    /// </summary>
    public string? Puesto { get; set; }

    // Navegación
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Empresa? Empresa { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Area? Area { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Usuario? Usuario { get; set; }
}
