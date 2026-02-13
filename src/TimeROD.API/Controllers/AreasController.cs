using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeROD.Core.Entities;
using TimeROD.Infrastructure.Data;

namespace TimeROD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AreasController : ControllerBase
{
    private readonly TimeRODDbContext _context;
    private readonly ILogger<AreasController> _logger;

    public AreasController(TimeRODDbContext context, ILogger<AreasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las áreas activas
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Area>>> GetAreas()
    {
        try
        {
            var areas = await _context.Areas
                .Include(a => a.Empresa)
                .Include(a => a.Supervisor)
                .Where(a => a.Activa)
                .OrderBy(a => a.Nombre)
                .ToListAsync();

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
    public async Task<ActionResult<Area>> GetArea(int id)
    {
        try
        {
            var area = await _context.Areas
                .Include(a => a.Empresa)
                .Include(a => a.Supervisor)
                .Include(a => a.Empleados)
                .FirstOrDefaultAsync(a => a.Id == id);

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
    public async Task<ActionResult<IEnumerable<Area>>> GetAreasByEmpresa(int empresaId)
    {
        try
        {
            var areas = await _context.Areas
                .Include(a => a.Supervisor)
                .Where(a => a.EmpresaId == empresaId && a.Activa)
                .OrderBy(a => a.Nombre)
                .ToListAsync();

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
    public async Task<ActionResult<Area>> PostArea(Area area)
    {
        try
        {
            // Validar que la empresa existe
            var empresaExiste = await _context.Empresas.AnyAsync(e => e.Id == area.EmpresaId);
            if (!empresaExiste)
            {
                return BadRequest(new { error = $"Empresa con ID {area.EmpresaId} no encontrada" });
            }

            // Validar que el supervisor existe (si se proporcionó)
            if (area.SupervisorId.HasValue)
            {
                var supervisorExiste = await _context.Usuarios
                    .AnyAsync(u => u.Id == area.SupervisorId.Value && u.Activo);

                if (!supervisorExiste)
                {
                    return BadRequest(new { error = $"Supervisor con ID {area.SupervisorId} no encontrado" });
                }
            }

            _context.Areas.Add(area);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArea), new { id = area.Id }, area);
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
    public async Task<IActionResult> PutArea(int id, Area area)
    {
        if (id != area.Id)
        {
            return BadRequest(new { error = "ID en URL no coincide con ID del área" });
        }

        try
        {
            var areaExistente = await _context.Areas.FindAsync(id);

            if (areaExistente == null)
            {
                return NotFound(new { error = $"Área con ID {id} no encontrada" });
            }

            // Validar supervisor si se proporcionó
            if (area.SupervisorId.HasValue)
            {
                var supervisorExiste = await _context.Usuarios
                    .AnyAsync(u => u.Id == area.SupervisorId.Value && u.Activo);

                if (!supervisorExiste)
                {
                    return BadRequest(new { error = $"Supervisor con ID {area.SupervisorId} no encontrado" });
                }
            }

            // Actualizar campos
            areaExistente.Nombre = area.Nombre;
            areaExistente.Descripcion = area.Descripcion;
            areaExistente.SupervisorId = area.SupervisorId;
            areaExistente.Activa = area.Activa;
            areaExistente.EmpresaId = area.EmpresaId;

            await _context.SaveChangesAsync();

            return NoContent();
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
            var area = await _context.Areas
                .Include(a => a.Empleados)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (area == null)
            {
                return NotFound(new { error = $"Área con ID {id} no encontrada" });
            }

            // Verificar si tiene empleados activos
            var empleadosActivos = area.Empleados.Count(e => e.Activo);
            if (empleadosActivos > 0)
            {
                return BadRequest(new
                {
                    error = $"No se puede desactivar el área porque tiene {empleadosActivos} empleado(s) activo(s)",
                    empleadosActivos
                });
            }

            // Soft delete: solo marcar como inactiva
            area.Activa = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar área {AreaId}", id);
            return StatusCode(500, new { error = "Error al desactivar área", detalle = ex.Message });
        }
    }
}
