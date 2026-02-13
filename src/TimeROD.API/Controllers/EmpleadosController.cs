using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeROD.Core.Entities;
using TimeROD.Infrastructure.Data;

namespace TimeROD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmpleadosController : ControllerBase
{
    private readonly TimeRODDbContext _context;
    private readonly ILogger<EmpleadosController> _logger;

    public EmpleadosController(TimeRODDbContext context, ILogger<EmpleadosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los empleados activos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Empleado>>> GetEmpleados()
    {
        try
        {
            var empleados = await _context.Empleados
                .Include(e => e.Empresa)
                .Include(e => e.Area)
                .Include(e => e.Usuario)
                .Where(e => e.Activo)
                .OrderBy(e => e.Apellidos)
                .ThenBy(e => e.Nombre)
                .ToListAsync();

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
    public async Task<ActionResult<Empleado>> GetEmpleado(int id)
    {
        try
        {
            var empleado = await _context.Empleados
                .Include(e => e.Empresa)
                .Include(e => e.Area)
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(e => e.Id == id);

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
    public async Task<ActionResult<IEnumerable<Empleado>>> GetEmpleadosByEmpresa(int empresaId)
    {
        try
        {
            var empleados = await _context.Empleados
                .Include(e => e.Area)
                .Include(e => e.Usuario)
                .Where(e => e.EmpresaId == empresaId && e.Activo)
                .OrderBy(e => e.Apellidos)
                .ThenBy(e => e.Nombre)
                .ToListAsync();

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
    public async Task<ActionResult<IEnumerable<Empleado>>> GetEmpleadosByArea(int areaId)
    {
        try
        {
            var empleados = await _context.Empleados
                .Include(e => e.Usuario)
                .Where(e => e.AreaId == areaId && e.Activo)
                .OrderBy(e => e.Apellidos)
                .ThenBy(e => e.Nombre)
                .ToListAsync();

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
    public async Task<ActionResult<Empleado>> GetEmpleadoByNumero(string numeroEmpleado)
    {
        try
        {
            var empleado = await _context.Empleados
                .Include(e => e.Empresa)
                .Include(e => e.Area)
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(e => e.NumeroEmpleado == numeroEmpleado);

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
    public async Task<ActionResult<Empleado>> PostEmpleado(Empleado empleado)
    {
        try
        {
            // Validar que la empresa existe
            var empresaExiste = await _context.Empresas.AnyAsync(e => e.Id == empleado.EmpresaId);
            if (!empresaExiste)
            {
                return BadRequest(new { error = $"Empresa con ID {empleado.EmpresaId} no encontrada" });
            }

            // Validar que el área existe
            var areaExiste = await _context.Areas
                .AnyAsync(a => a.Id == empleado.AreaId && a.EmpresaId == empleado.EmpresaId);
            if (!areaExiste)
            {
                return BadRequest(new { error = $"Área con ID {empleado.AreaId} no encontrada o no pertenece a la empresa" });
            }

            // Validar que el número de empleado no exista
            var numeroExiste = await _context.Empleados
                .AnyAsync(e => e.NumeroEmpleado == empleado.NumeroEmpleado && e.EmpresaId == empleado.EmpresaId);
            if (numeroExiste)
            {
                return BadRequest(new { error = $"Número de empleado {empleado.NumeroEmpleado} ya existe en esta empresa" });
            }

            // Validar usuario si se proporcionó
            if (empleado.UsuarioId.HasValue)
            {
                var usuarioExiste = await _context.Usuarios
                    .AnyAsync(u => u.Id == empleado.UsuarioId.Value && u.EmpresaId == empleado.EmpresaId);
                if (!usuarioExiste)
                {
                    return BadRequest(new { error = $"Usuario con ID {empleado.UsuarioId} no encontrado o no pertenece a la empresa" });
                }
            }

            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmpleado), new { id = empleado.Id }, empleado);
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
    public async Task<IActionResult> PutEmpleado(int id, Empleado empleado)
    {
        if (id != empleado.Id)
        {
            return BadRequest(new { error = "ID en URL no coincide con ID del empleado" });
        }

        try
        {
            var empleadoExistente = await _context.Empleados.FindAsync(id);

            if (empleadoExistente == null)
            {
                return NotFound(new { error = $"Empleado con ID {id} no encontrado" });
            }

            // Validar que el área existe
            var areaExiste = await _context.Areas
                .AnyAsync(a => a.Id == empleado.AreaId && a.EmpresaId == empleado.EmpresaId);
            if (!areaExiste)
            {
                return BadRequest(new { error = $"Área con ID {empleado.AreaId} no encontrada o no pertenece a la empresa" });
            }

            // Validar número de empleado único (excepto el mismo)
            var numeroExiste = await _context.Empleados
                .AnyAsync(e => e.NumeroEmpleado == empleado.NumeroEmpleado &&
                              e.EmpresaId == empleado.EmpresaId &&
                              e.Id != id);
            if (numeroExiste)
            {
                return BadRequest(new { error = $"Número de empleado {empleado.NumeroEmpleado} ya existe en esta empresa" });
            }

            // Validar usuario si se proporcionó
            if (empleado.UsuarioId.HasValue)
            {
                var usuarioExiste = await _context.Usuarios
                    .AnyAsync(u => u.Id == empleado.UsuarioId.Value && u.EmpresaId == empleado.EmpresaId);
                if (!usuarioExiste)
                {
                    return BadRequest(new { error = $"Usuario con ID {empleado.UsuarioId} no encontrado o no pertenece a la empresa" });
                }
            }

            // Actualizar campos
            empleadoExistente.NumeroEmpleado = empleado.NumeroEmpleado;
            empleadoExistente.Nombre = empleado.Nombre;
            empleadoExistente.Apellidos = empleado.Apellidos;
            empleadoExistente.AreaId = empleado.AreaId;
            empleadoExistente.FechaIngreso = empleado.FechaIngreso;
            empleadoExistente.SalarioDiario = empleado.SalarioDiario;
            empleadoExistente.Puesto = empleado.Puesto;
            empleadoExistente.TurnoId = empleado.TurnoId;
            empleadoExistente.IdBiometrico = empleado.IdBiometrico;
            empleadoExistente.UsuarioId = empleado.UsuarioId;
            empleadoExistente.Activo = empleado.Activo;

            await _context.SaveChangesAsync();

            return NoContent();
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
            var empleado = await _context.Empleados.FindAsync(id);

            if (empleado == null)
            {
                return NotFound(new { error = $"Empleado con ID {id} no encontrado" });
            }

            // Soft delete: solo marcar como inactivo
            empleado.Activo = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar empleado {EmpleadoId}", id);
            return StatusCode(500, new { error = "Error al desactivar empleado", detalle = ex.Message });
        }
    }
}
