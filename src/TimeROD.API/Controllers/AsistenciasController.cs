using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeROD.Core.Entities;
using TimeROD.Infrastructure.Data;

namespace TimeROD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AsistenciasController : ControllerBase
{
    private readonly TimeRODDbContext _context;
    private readonly ILogger<AsistenciasController> _logger;

    public AsistenciasController(TimeRODDbContext context, ILogger<AsistenciasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las asistencias (con filtros opcionales)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Asistencia>>> GetAsistencias(
        [FromQuery] int? empleadoId = null,
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null)
    {
        try
        {
            var query = _context.Asistencias
                .Include(a => a.Empleado)
                    .ThenInclude(e => e.Area)
                .AsQueryable();

            // Filtrar por empleado si se proporciona
            if (empleadoId.HasValue)
            {
                query = query.Where(a => a.EmpleadoId == empleadoId.Value);
            }

            // Filtrar por rango de fechas
            if (fechaInicio.HasValue)
            {
                query = query.Where(a => a.Fecha >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                query = query.Where(a => a.Fecha <= fechaFin.Value);
            }

            var asistencias = await query
                .OrderByDescending(a => a.Fecha)
                .ThenByDescending(a => a.HoraEntrada)
                .ToListAsync();

            return Ok(asistencias);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener asistencias");
            return StatusCode(500, new { error = "Error al obtener asistencias", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene una asistencia por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Asistencia>> GetAsistencia(int id)
    {
        try
        {
            var asistencia = await _context.Asistencias
                .Include(a => a.Empleado)
                    .ThenInclude(e => e.Area)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (asistencia == null)
            {
                return NotFound(new { error = $"Asistencia con ID {id} no encontrada" });
            }

            return Ok(asistencia);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener asistencia {AsistenciaId}", id);
            return StatusCode(500, new { error = "Error al obtener asistencia", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene asistencias de un empleado específico
    /// </summary>
    [HttpGet("empleado/{empleadoId}")]
    public async Task<ActionResult<IEnumerable<Asistencia>>> GetAsistenciasByEmpleado(
        int empleadoId,
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null)
    {
        try
        {
            var query = _context.Asistencias
                .Where(a => a.EmpleadoId == empleadoId)
                .AsQueryable();

            if (fechaInicio.HasValue)
            {
                query = query.Where(a => a.Fecha >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                query = query.Where(a => a.Fecha <= fechaFin.Value);
            }

            var asistencias = await query
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();

            return Ok(asistencias);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener asistencias del empleado {EmpleadoId}", empleadoId);
            return StatusCode(500, new { error = "Error al obtener asistencias del empleado", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Registra la entrada de un empleado
    /// </summary>
    [HttpPost("entrada")]
    public async Task<ActionResult<Asistencia>> RegistrarEntrada([FromBody] RegistroEntradaDto dto)
    {
        try
        {
            // Validar que el empleado existe y está activo
            var empleado = await _context.Empleados.FindAsync(dto.EmpleadoId);
            if (empleado == null || !empleado.Activo)
            {
                return BadRequest(new { error = "Empleado no encontrado o inactivo" });
            }

            var fechaHoy = DateTime.UtcNow.Date;

            // Verificar si ya tiene una entrada hoy
            var asistenciaExistente = await _context.Asistencias
                .FirstOrDefaultAsync(a => a.EmpleadoId == dto.EmpleadoId && a.Fecha == fechaHoy);

            if (asistenciaExistente != null && asistenciaExistente.HoraEntrada != null)
            {
                return BadRequest(new
                {
                    error = "Ya existe un registro de entrada para hoy",
                    asistencia = asistenciaExistente
                });
            }

            // Si existe un registro pero sin hora de entrada, actualizarlo
            if (asistenciaExistente != null)
            {
                asistenciaExistente.HoraEntrada = DateTime.UtcNow;
                asistenciaExistente.Notas = dto.Notas;
                await _context.SaveChangesAsync();

                return Ok(asistenciaExistente);
            }

            // Crear nuevo registro de asistencia
            var asistencia = new Asistencia
            {
                EmpleadoId = dto.EmpleadoId,
                Fecha = fechaHoy,
                HoraEntrada = DateTime.UtcNow,
                Tipo = TipoAsistencia.Normal,
                Notas = dto.Notas,
                Aprobado = true
            };

            // TODO: Implementar lógica de llegada tardía
            // Por ahora, marcar como puntual
            asistencia.LlegadaTardia = false;
            asistencia.MinutosRetraso = 0;

            _context.Asistencias.Add(asistencia);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAsistencia), new { id = asistencia.Id }, asistencia);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar entrada");
            return StatusCode(500, new { error = "Error al registrar entrada", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Registra la salida de un empleado
    /// </summary>
    [HttpPost("salida")]
    public async Task<ActionResult<Asistencia>> RegistrarSalida([FromBody] RegistroSalidaDto dto)
    {
        try
        {
            var fechaHoy = DateTime.UtcNow.Date;

            // Buscar el registro de asistencia del día
            var asistencia = await _context.Asistencias
                .FirstOrDefaultAsync(a => a.EmpleadoId == dto.EmpleadoId && a.Fecha == fechaHoy);

            if (asistencia == null)
            {
                return BadRequest(new { error = "No existe un registro de entrada para hoy" });
            }

            if (asistencia.HoraEntrada == null)
            {
                return BadRequest(new { error = "No se ha registrado la hora de entrada" });
            }

            if (asistencia.HoraSalida != null)
            {
                return BadRequest(new
                {
                    error = "Ya existe un registro de salida para hoy",
                    asistencia
                });
            }

            // Registrar hora de salida
            asistencia.HoraSalida = DateTime.UtcNow;
            asistencia.Notas = string.IsNullOrEmpty(dto.Notas)
                ? asistencia.Notas
                : $"{asistencia.Notas} | {dto.Notas}";

            // Calcular horas trabajadas
            var duracion = asistencia.HoraSalida.Value - asistencia.HoraEntrada.Value;
            asistencia.HorasTrabajadas = (decimal)duracion.TotalHours;

            await _context.SaveChangesAsync();

            return Ok(asistencia);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar salida");
            return StatusCode(500, new { error = "Error al registrar salida", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene reporte de asistencias por fecha
    /// </summary>
    [HttpGet("reporte")]
    public async Task<ActionResult> GetReporteAsistencias(
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null,
        [FromQuery] int? empresaId = null)
    {
        try
        {
            // Por defecto, últimos 30 días
            fechaInicio ??= DateTime.UtcNow.AddDays(-30).Date;
            fechaFin ??= DateTime.UtcNow.Date;

            var query = _context.Asistencias
                .Include(a => a.Empleado)
                    .ThenInclude(e => e.Area)
                .Include(a => a.Empleado)
                    .ThenInclude(e => e.Empresa)
                .Where(a => a.Fecha >= fechaInicio && a.Fecha <= fechaFin);

            if (empresaId.HasValue)
            {
                query = query.Where(a => a.Empleado.EmpresaId == empresaId.Value);
            }

            var asistencias = await query.ToListAsync();

            var reporte = new
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                TotalRegistros = asistencias.Count,
                TotalHorasTrabajadas = asistencias.Sum(a => a.HorasTrabajadas ?? 0),
                PromedioHorasPorDia = asistencias.Any()
                    ? asistencias.Average(a => a.HorasTrabajadas ?? 0)
                    : 0,
                LlegadasTardias = asistencias.Count(a => a.LlegadaTardia),
                Asistencias = asistencias
                    .OrderByDescending(a => a.Fecha)
                    .Select(a => new
                    {
                        a.Id,
                        a.Fecha,
                        a.HoraEntrada,
                        a.HoraSalida,
                        a.HorasTrabajadas,
                        a.LlegadaTardia,
                        a.MinutosRetraso,
                        a.Tipo,
                        Empleado = new
                        {
                            a.Empleado.Id,
                            a.Empleado.NumeroEmpleado,
                            NombreCompleto = $"{a.Empleado.Nombre} {a.Empleado.Apellidos}",
                            Area = a.Empleado.Area.Nombre,
                            Empresa = a.Empleado.Empresa.Nombre
                        }
                    })
            };

            return Ok(reporte);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar reporte de asistencias");
            return StatusCode(500, new { error = "Error al generar reporte", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza una asistencia existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsistencia(int id, Asistencia asistencia)
    {
        if (id != asistencia.Id)
        {
            return BadRequest(new { error = "ID en URL no coincide con ID de la asistencia" });
        }

        try
        {
            var asistenciaExistente = await _context.Asistencias.FindAsync(id);

            if (asistenciaExistente == null)
            {
                return NotFound(new { error = $"Asistencia con ID {id} no encontrada" });
            }

            // Actualizar campos
            asistenciaExistente.HoraEntrada = asistencia.HoraEntrada;
            asistenciaExistente.HoraSalida = asistencia.HoraSalida;
            asistenciaExistente.Tipo = asistencia.Tipo;
            asistenciaExistente.Notas = asistencia.Notas;
            asistenciaExistente.Aprobado = asistencia.Aprobado;
            asistenciaExistente.LlegadaTardia = asistencia.LlegadaTardia;
            asistenciaExistente.MinutosRetraso = asistencia.MinutosRetraso;

            // Recalcular horas trabajadas si hay entrada y salida
            if (asistenciaExistente.HoraEntrada.HasValue && asistenciaExistente.HoraSalida.HasValue)
            {
                var duracion = asistenciaExistente.HoraSalida.Value - asistenciaExistente.HoraEntrada.Value;
                asistenciaExistente.HorasTrabajadas = (decimal)duracion.TotalHours;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar asistencia {AsistenciaId}", id);
            return StatusCode(500, new { error = "Error al actualizar asistencia", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Elimina una asistencia
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsistencia(int id)
    {
        try
        {
            var asistencia = await _context.Asistencias.FindAsync(id);

            if (asistencia == null)
            {
                return NotFound(new { error = $"Asistencia con ID {id} no encontrada" });
            }

            _context.Asistencias.Remove(asistencia);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar asistencia {AsistenciaId}", id);
            return StatusCode(500, new { error = "Error al eliminar asistencia", detalle = ex.Message });
        }
    }
}

/// <summary>
/// DTO para registrar entrada
/// </summary>
public class RegistroEntradaDto
{
    public int EmpleadoId { get; set; }
    public string? Notas { get; set; }
}

/// <summary>
/// DTO para registrar salida
/// </summary>
public class RegistroSalidaDto
{
    public int EmpleadoId { get; set; }
    public string? Notas { get; set; }
}
