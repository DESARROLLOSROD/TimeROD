using System.Text.Json.Serialization;

namespace TimeROD.Core.Entities;

/// <summary>
/// Usuario del sistema con acceso a la aplicación web
/// </summary>
public class Usuario : BaseEntity
{
    /// <summary>
    /// ID de la empresa a la que pertenece (multi-tenant)
    /// </summary>
    public int EmpresaId { get; set; }

    /// <summary>
    /// Email único del usuario (usado para login)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash de la contraseña (nunca guardar en texto plano)
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Nombre completo del usuario
    /// </summary>
    public string NombreCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Rol del usuario en el sistema
    /// </summary>
    public RolUsuario Rol { get; set; } = RolUsuario.Empleado;

    /// <summary>
    /// Indica si el usuario está activo
    /// </summary>
    public bool Activo { get; set; } = true;

    /// <summary>
    /// Última vez que el usuario inició sesión
    /// </summary>
    public DateTime? UltimoAcceso { get; set; }

    // Navegación
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Empresa? Empresa { get; set; }
}

/// <summary>
/// Roles disponibles en el sistema
/// </summary>
public enum RolUsuario
{
    /// <summary>
    /// Administrador del sistema (puede gestionar empresas)
    /// </summary>
    Admin = 1,

    /// <summary>
    /// Recursos Humanos (gestiona empleados, turnos, pre-nómina)
    /// </summary>
    RH = 2,

    /// <summary>
    /// Supervisor (aprueba incidencias de su equipo)
    /// </summary>
    Supervisor = 3,

    /// <summary>
    /// Empleado regular (registra asistencia, solicita incidencias)
    /// </summary>
    Empleado = 4,

    /// <summary>
    /// Encargado de comedor (ve reportes de comedor)
    /// </summary>
    Comedor = 5
}
