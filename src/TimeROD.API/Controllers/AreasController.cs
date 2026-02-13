using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeROD.Core.DTOs;
using TimeROD.Core.Interfaces;

namespace TimeROD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AreasController : ControllerBase
{
    private readonly IAreaService _areaService;
    private readonly ILogger<AreasController> _logger;

    public AreasController(IAreaService areaService, ILogger<AreasController> logger)
    {
        _areaService = areaService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las áreas activas
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AreaDto>>> GetAreas()
    {
        try
        {
            var areas = await _areaService.GetAllAsync();
            return Ok(areas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener áreas");
            return StatusCode(500, new { error = "Error al obtener áreas", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene un área por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AreaDto>> GetArea(int id)
    {
        try
        {
            var area = await _areaService.GetByIdAsync(id);

            if (area == null)
            {
                return NotFound(new { error = $"Área con ID {id} no encontrada" });
            }

            return Ok(area);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener área {AreaId}", id);
            return StatusCode(500, new { error = "Error al obtener área", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene todas las áreas de una empresa
    /// </summary>
    [HttpGet("empresa/{empresaId}")]
    public async Task<ActionResult<IEnumerable<AreaDto>>> GetAreasByEmpresa(int empresaId)
    {
        try
        {
            var areas = await _areaService.GetAllByEmpresaAsync(empresaId);
            return Ok(areas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener áreas de empresa {EmpresaId}", empresaId);
            return StatusCode(500, new { error = "Error al obtener áreas de empresa", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Crea una nueva área
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AreaDto>> PostArea(CreateAreaDto dto)
    {
        try
        {
            var nuevaArea = await _areaService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetArea), new { id = nuevaArea.Id }, nuevaArea);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear área");
            return StatusCode(500, new { error = "Error al crear área", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza un área existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutArea(int id, UpdateAreaDto dto)
    {
        try
        {
            await _areaService.UpdateAsync(id, dto);
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
            _logger.LogError(ex, "Error al actualizar área {AreaId}", id);
            return StatusCode(500, new { error = "Error al actualizar área", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Desactiva un área (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArea(int id)
    {
        try
        {
            await _areaService.DeleteAsync(id);
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
            _logger.LogError(ex, "Error al desactivar área {AreaId}", id);
            return StatusCode(500, new { error = "Error al desactivar área", detalle = ex.Message });
        }
    }
}
