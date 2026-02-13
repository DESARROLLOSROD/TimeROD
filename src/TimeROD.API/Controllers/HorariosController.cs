using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeROD.Core.DTOs;
using TimeROD.Core.Interfaces;

namespace TimeROD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HorariosController : ControllerBase
{
    private readonly IHorarioService _horarioService;
    private readonly ILogger<HorariosController> _logger;

    public HorariosController(IHorarioService horarioService, ILogger<HorariosController> logger)
    {
        _horarioService = horarioService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HorarioDto>>> GetHorarios()
    {
        try
        {
            var horarios = await _horarioService.GetAllAsync();
            return Ok(horarios);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener horarios");
            return StatusCode(500, new { error = "Error al obtener horarios", detalle = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HorarioDto>> GetHorario(int id)
    {
        try
        {
            var horario = await _horarioService.GetByIdAsync(id);

            if (horario == null)
            {
                return NotFound(new { error = $"Horario con ID {id} no encontrado" });
            }

            return Ok(horario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener horario {HorarioId}", id);
            return StatusCode(500, new { error = "Error al obtener horario", detalle = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<HorarioDto>> PostHorario(CreateHorarioDto dto)
    {
        try
        {
            var nuevoHorario = await _horarioService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetHorario), new { id = nuevoHorario.Id }, nuevoHorario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear horario");
            return StatusCode(500, new { error = "Error al crear horario", detalle = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutHorario(int id, UpdateHorarioDto dto)
    {
        try
        {
            await _horarioService.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar horario {HorarioId}", id);
            return StatusCode(500, new { error = "Error al actualizar horario", detalle = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHorario(int id)
    {
        try
        {
            await _horarioService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar horario {HorarioId}", id);
            return StatusCode(500, new { error = "Error al desactivar horario", detalle = ex.Message });
        }
    }
}
