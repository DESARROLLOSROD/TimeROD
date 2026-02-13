using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeROD.Core.Entities;
using TimeROD.Infrastructure.Data;
using TimeROD.Core.Interfaces;
using TimeROD.Core.DTOs;

namespace TimeROD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(IUsuarioService usuarioService, ILogger<UsuariosController> logger)
    {
        _usuarioService = usuarioService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los usuarios activos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsuarios()
    {
        try
        {
            var usuarios = await _usuarioService.GetAllAsync();
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
    public async Task<ActionResult<UsuarioDto>> GetUsuario(int id)
    {
        try
        {
            var usuario = await _usuarioService.GetByIdAsync(id);

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
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsuariosByEmpresa(int empresaId)
    {
        try
        {
            var usuarios = await _usuarioService.GetAllAsync(empresaId);
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
    public async Task<ActionResult<UsuarioDto>> PostUsuario(CreateUsuarioDto usuarioDto)
    {
        try
        {
            var nuevoUsuario = await _usuarioService.CreateAsync(usuarioDto);
            return CreatedAtAction(nameof(GetUsuario), new { id = nuevoUsuario.Id }, nuevoUsuario);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
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
    public async Task<IActionResult> PutUsuario(int id, UpdateUsuarioDto usuarioDto)
    {
        try
        {
            await _usuarioService.UpdateAsync(id, usuarioDto);
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
            await _usuarioService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar usuario {UsuarioId}", id);
            return StatusCode(500, new { error = "Error al desactivar usuario", detalle = ex.Message });
        }
    }
}
