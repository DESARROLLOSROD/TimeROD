using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeROD.Core.DTOs;
using TimeROD.Core.Interfaces;

namespace TimeROD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AsistenciasController : ControllerBase
{
    private readonly IAsistenciaService _asistenciaService;
    private readonly ILogger<AsistenciasController> _logger;

    public AsistenciasController(IAsistenciaService asistenciaService, ILogger<AsistenciasController> logger)
    {
        _asistenciaService = asistenciaService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las asistencias (con filtros opcionales)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AsistenciaDto>>> GetAsistencias(
        [FromQuery] int? empleadoId = null,
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null)
    {
        try
        {
            var asistencias = await _asistenciaService.GetAllAsync(empleadoId, fechaInicio, fechaFin);
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
    public async Task<ActionResult<AsistenciaDto>> GetAsistencia(int id)
    {
        try
        {
            var asistencia = await _asistenciaService.GetByIdAsync(id);

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
    public async Task<ActionResult<IEnumerable<AsistenciaDto>>> GetAsistenciasByEmpleado(
        int empleadoId,
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null)
    {
        try
        {
            var asistencias = await _asistenciaService.GetByEmpleadoAsync(empleadoId, fechaInicio, fechaFin);
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
    public async Task<ActionResult<AsistenciaDto>> RegistrarEntrada([FromBody] RegistroEntradaDto dto)
    {
        try
        {
            var asistencia = await _asistenciaService.RegistrarEntradaAsync(dto);
            return CreatedAtAction(nameof(GetAsistencia), new { id = asistencia.Id }, asistencia);
        }
        catch (InvalidOperationException ex)
        {
             return BadRequest(new { error = ex.Message });
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
    public async Task<ActionResult<AsistenciaDto>> RegistrarSalida([FromBody] RegistroSalidaDto dto)
    {
        try
        {
            var asistencia = await _asistenciaService.RegistrarSalidaAsync(dto);
            return Ok(asistencia);
        }
        catch (InvalidOperationException ex)
        {
             return BadRequest(new { error = ex.Message });
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
             var reporte = await _asistenciaService.GetReporteAsync(fechaInicio, fechaFin, empresaId);
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
    public async Task<IActionResult> PutAsistencia(int id, UpdateAsistenciaDto dto)
    {
        try
        {
            await _asistenciaService.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
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
            await _asistenciaService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar asistencia {AsistenciaId}", id);
            return StatusCode(500, new { error = "Error al eliminar asistencia", detalle = ex.Message });
        }
    }
}


