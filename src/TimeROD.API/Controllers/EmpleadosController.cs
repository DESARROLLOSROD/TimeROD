using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeROD.Core.DTOs;
using TimeROD.Core.Interfaces;

namespace TimeROD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmpleadosController : ControllerBase
{
    private readonly IEmpleadoService _empleadoService;
    private readonly ILogger<EmpleadosController> _logger;

    public EmpleadosController(IEmpleadoService empleadoService, ILogger<EmpleadosController> logger)
    {
        _empleadoService = empleadoService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los empleados activos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmpleadoDto>>> GetEmpleados()
    {
        try
        {
            var empleados = await _empleadoService.GetAllAsync();
            return Ok(empleados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener empleados");
            return StatusCode(500, new { error = "Error al obtener empleados", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene un empleado por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EmpleadoDto>> GetEmpleado(int id)
    {
        try
        {
            var empleado = await _empleadoService.GetByIdAsync(id);

            if (empleado == null)
            {
                return NotFound(new { error = $"Empleado con ID {id} no encontrado" });
            }

            return Ok(empleado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener empleado {EmpleadoId}", id);
            return StatusCode(500, new { error = "Error al obtener empleado", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene empleados por empresa
    /// </summary>
    [HttpGet("empresa/{empresaId}")]
    public async Task<ActionResult<IEnumerable<EmpleadoDto>>> GetEmpleadosByEmpresa(int empresaId)
    {
        try
        {
            var empleados = await _empleadoService.GetAllByEmpresaAsync(empresaId);
            return Ok(empleados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener empleados de empresa {EmpresaId}", empresaId);
            return StatusCode(500, new { error = "Error al obtener empleados de empresa", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene empleados por área
    /// </summary>
    [HttpGet("area/{areaId}")]
    public async Task<ActionResult<IEnumerable<EmpleadoDto>>> GetEmpleadosByArea(int areaId)
    {
        try
        {
            var empleados = await _empleadoService.GetAllByAreaAsync(areaId);
            return Ok(empleados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener empleados de área {AreaId}", areaId);
            return StatusCode(500, new { error = "Error al obtener empleados de área", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Busca empleado por número de empleado
    /// </summary>
    [HttpGet("numero/{numeroEmpleado}")]
    public async Task<ActionResult<EmpleadoDto>> GetEmpleadoByNumero(string numeroEmpleado)
    {
        try
        {
            var empleado = await _empleadoService.GetByNumeroEmpleadoAsync(numeroEmpleado);

            if (empleado == null)
            {
                return NotFound(new { error = $"Empleado con número {numeroEmpleado} no encontrado" });
            }

            return Ok(empleado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener empleado por número {NumeroEmpleado}", numeroEmpleado);
            return StatusCode(500, new { error = "Error al obtener empleado por número", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Crea un nuevo empleado
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EmpleadoDto>> PostEmpleado(CreateEmpleadoDto dto)
    {
        try
        {
            var nuevoEmpleado = await _empleadoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetEmpleado), new { id = nuevoEmpleado.Id }, nuevoEmpleado);
        }
        catch (InvalidOperationException ex)
        {
             return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear empleado");
            return StatusCode(500, new { error = "Error al crear empleado", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza un empleado existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEmpleado(int id, UpdateEmpleadoDto dto)
    {
        try
        {
             await _empleadoService.UpdateAsync(id, dto);
             return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
             return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
             return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar empleado {EmpleadoId}", id);
            return StatusCode(500, new { error = "Error al actualizar empleado", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Desactiva un empleado (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmpleado(int id)
    {
        try
        {
            await _empleadoService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
             return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar empleado {EmpleadoId}", id);
            return StatusCode(500, new { error = "Error al desactivar empleado", detalle = ex.Message });
        }
    }
}
