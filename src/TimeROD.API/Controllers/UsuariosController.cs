using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeROD.Core.Entities;
using TimeROD.Infrastructure.Data;

namespace TimeROD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly TimeRODDbContext _context;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(TimeRODDbContext context, ILogger<UsuariosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los usuarios activos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
    {
        try
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Empresa)
                .Where(u => u.Activo)
                .OrderBy(u => u.NombreCompleto)
                .ToListAsync();

            return Ok(usuarios);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios");
            return StatusCode(500, new { error = "Error al obtener usuarios", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene un usuario por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Usuario>> GetUsuario(int id)
    {
        try
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Empresa)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return NotFound(new { error = $"Usuario con ID {id} no encontrado" });
            }

            return Ok(usuario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario {UsuarioId}", id);
            return StatusCode(500, new { error = "Error al obtener usuario", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene todos los usuarios de una empresa
    /// </summary>
    [HttpGet("empresa/{empresaId}")]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuariosByEmpresa(int empresaId)
    {
        try
        {
            var usuarios = await _context.Usuarios
                .Where(u => u.EmpresaId == empresaId && u.Activo)
                .OrderBy(u => u.NombreCompleto)
                .ToListAsync();

            return Ok(usuarios);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios de empresa {EmpresaId}", empresaId);
            return StatusCode(500, new { error = "Error al obtener usuarios de empresa", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Crea un nuevo usuario
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
    {
        try
        {
            // Validar que el email no exista
            var emailExiste = await _context.Usuarios
                .AnyAsync(u => u.Email == usuario.Email);

            if (emailExiste)
            {
                return BadRequest(new { error = $"El email {usuario.Email} ya está registrado" });
            }

            // Validar y hashear password
            if (string.IsNullOrEmpty(usuario.PasswordHash))
            {
                return BadRequest(new { error = "Password es requerido" });
            }

            // Hashear password con BCrypt
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario");
            return StatusCode(500, new { error = "Error al crear usuario", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza un usuario existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
    {
        if (id != usuario.Id)
        {
            return BadRequest(new { error = "ID en URL no coincide con ID del usuario" });
        }

        try
        {
            var usuarioExistente = await _context.Usuarios.FindAsync(id);

            if (usuarioExistente == null)
            {
                return NotFound(new { error = $"Usuario con ID {id} no encontrado" });
            }

            // Validar que el email no esté en uso por otro usuario
            var emailExiste = await _context.Usuarios
                .AnyAsync(u => u.Email == usuario.Email && u.Id != id);

            if (emailExiste)
            {
                return BadRequest(new { error = $"El email {usuario.Email} ya está registrado por otro usuario" });
            }

            // Actualizar campos
            usuarioExistente.Email = usuario.Email;
            usuarioExistente.NombreCompleto = usuario.NombreCompleto;
            usuarioExistente.Rol = usuario.Rol;
            usuarioExistente.Activo = usuario.Activo;
            usuarioExistente.EmpresaId = usuario.EmpresaId;

            // Solo actualizar password si viene uno nuevo
            if (!string.IsNullOrEmpty(usuario.PasswordHash) &&
                usuario.PasswordHash != usuarioExistente.PasswordHash)
            {
                // Hashear el nuevo password con BCrypt
                usuarioExistente.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario {UsuarioId}", id);
            return StatusCode(500, new { error = "Error al actualizar usuario", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Desactiva un usuario (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUsuario(int id)
    {
        try
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound(new { error = $"Usuario con ID {id} no encontrado" });
            }

            // Soft delete: solo marcar como inactivo
            usuario.Activo = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar usuario {UsuarioId}", id);
            return StatusCode(500, new { error = "Error al desactivar usuario", detalle = ex.Message });
        }
    }
}
