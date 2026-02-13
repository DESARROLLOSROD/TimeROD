using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeROD.Core.DTOs;
using TimeROD.Core.Interfaces;

namespace TimeROD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmpresasController : ControllerBase
{
    private readonly IEmpresaService _empresaService;
    private readonly ILogger<EmpresasController> _logger;

    public EmpresasController(IEmpresaService empresaService, ILogger<EmpresasController> logger)
    {
        _empresaService = empresaService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las empresas
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmpresaDto>>> GetEmpresas()
    {
        try
        {
            var empresas = await _empresaService.GetAllAsync();
            return Ok(empresas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener empresas");
            return StatusCode(500, new { error = "Error al obtener empresas", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene una empresa por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EmpresaDto>> GetEmpresa(int id)
    {
        try
        {
            var empresa = await _empresaService.GetByIdAsync(id);

            if (empresa == null)
            {
                return NotFound(new { error = $"Empresa con ID {id} no encontrada" });
            }

            return Ok(empresa);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener empresa {EmpresaId}", id);
            return StatusCode(500, new { error = "Error al obtener empresa", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Crea una nueva empresa
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EmpresaDto>> PostEmpresa(CreateEmpresaDto dto)
    {
        try
        {
            var nuevaEmpresa = await _empresaService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetEmpresa), new { id = nuevaEmpresa.Id }, nuevaEmpresa);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear empresa");
            return StatusCode(500, new { error = "Error al crear empresa", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza una empresa existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEmpresa(int id, UpdateEmpresaDto dto)
    {
        try
        {
            await _empresaService.UpdateAsync(id, dto);
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
            _logger.LogError(ex, "Error al actualizar empresa {EmpresaId}", id);
            return StatusCode(500, new { error = "Error al actualizar empresa", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Desactiva una empresa (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmpresa(int id)
    {
        try
        {
            await _empresaService.DeleteAsync(id);
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
            _logger.LogError(ex, "Error al desactivar empresa {EmpresaId}", id);
            return StatusCode(500, new { error = "Error al desactivar empresa", detalle = ex.Message });
        }
    }
}
