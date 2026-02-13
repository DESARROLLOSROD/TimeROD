using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeROD.Core.Entities;
using TimeROD.Infrastructure.Data;

namespace TimeROD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpresasController : ControllerBase
{
    private readonly TimeRODDbContext _context;
    private readonly ILogger<EmpresasController> _logger;

    public EmpresasController(TimeRODDbContext context, ILogger<EmpresasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las empresas
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Empresa>>> GetEmpresas()
    {
        try
        {
            var empresas = await _context.Empresas
                .Where(e => e.Activa)
                .OrderBy(e => e.Nombre)
                .ToListAsync();

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
    public async Task<ActionResult<Empresa>> GetEmpresa(int id)
    {
        try
        {
            var empresa = await _context.Empresas.FindAsync(id);

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
    public async Task<ActionResult<Empresa>> PostEmpresa(Empresa empresa)
    {
        try
        {
            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmpresa), new { id = empresa.Id }, empresa);
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
    public async Task<IActionResult> PutEmpresa(int id, Empresa empresa)
    {
        if (id != empresa.Id)
        {
            return BadRequest(new { error = "ID en URL no coincide con ID de la empresa" });
        }

        try
        {
            var empresaExistente = await _context.Empresas.FindAsync(id);

            if (empresaExistente == null)
            {
                return NotFound(new { error = $"Empresa con ID {id} no encontrada" });
            }

            // Validar RFC único (excepto la misma empresa)
            var rfcExiste = await _context.Empresas
                .AnyAsync(e => e.RFC == empresa.RFC && e.Id != id);

            if (rfcExiste)
            {
                return BadRequest(new { error = $"RFC {empresa.RFC} ya está registrado por otra empresa" });
            }

            // Actualizar campos
            empresaExistente.Nombre = empresa.Nombre;
            empresaExistente.RFC = empresa.RFC;
            empresaExistente.Direccion = empresa.Direccion;
            empresaExistente.ConfiguracionJson = empresa.ConfiguracionJson;
            empresaExistente.Activa = empresa.Activa;

            await _context.SaveChangesAsync();

            return NoContent();
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
            var empresa = await _context.Empresas
                .Include(e => e.Usuarios)
                .Include(e => e.Areas)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (empresa == null)
            {
                return NotFound(new { error = $"Empresa con ID {id} no encontrada" });
            }

            // Verificar si tiene usuarios o áreas activos
            var usuariosActivos = empresa.Usuarios.Count(u => u.Activo);
            var areasActivas = empresa.Areas.Count(a => a.Activa);

            if (usuariosActivos > 0 || areasActivas > 0)
            {
                return BadRequest(new
                {
                    error = "No se puede desactivar la empresa porque tiene usuarios o áreas activos",
                    usuariosActivos,
                    areasActivas
                });
            }

            // Soft delete: solo marcar como inactiva
            empresa.Activa = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar empresa {EmpresaId}", id);
            return StatusCode(500, new { error = "Error al desactivar empresa", detalle = ex.Message });
        }
    }
}
