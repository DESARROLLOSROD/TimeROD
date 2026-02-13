using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TimeROD.Infrastructure.Data;

namespace TimeROD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TimeRODDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        TimeRODDbContext context,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Login de usuario
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            // Validar datos de entrada
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { error = "Email y password son requeridos" });
            }

            // Buscar usuario por email
            var usuario = await _context.Usuarios
                .Include(u => u.Empresa)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (usuario == null)
            {
                _logger.LogWarning("Intento de login con email no encontrado: {Email}", request.Email);
                return Unauthorized(new { error = "Email o password incorrectos" });
            }

            // Verificar que el usuario esté activo
            if (!usuario.Activo)
            {
                _logger.LogWarning("Intento de login con usuario inactivo: {Email}", request.Email);
                return Unauthorized(new { error = "Usuario inactivo" });
            }

            // Verificar password con BCrypt
            bool passwordValido = false;
            bool needsRehash = false;

            try
            {
                passwordValido = BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash);
            }
            catch (Exception)
            {
                // Si falla (por ejemplo, formato inválido/salt version), intentamos comparar como texto plano (Legacy)
                if (usuario.PasswordHash == request.Password)
                {
                    passwordValido = true;
                    needsRehash = true;
                }
            }

            if (!passwordValido)
            {
                _logger.LogWarning("Intento de login con password incorrecto: {Email}", request.Email);
                return Unauthorized(new { error = "Email o password incorrectos" });
            }

            // Si es password legacy (texto plano), lo actualizamos a hash
            if (needsRehash)
            {
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                _logger.LogInformation("Password actualizado a hash para usuario: {Email}", request.Email);
            }

            // Actualizar último acceso
            usuario.UltimoAcceso = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Generar token JWT
            var token = GenerarTokenJwt(usuario);

            _logger.LogInformation("Login exitoso para usuario: {Email}", request.Email);

            return Ok(new LoginResponse
            {
                Token = token,
                Usuario = new UsuarioDto
                {
                    Id = usuario.Id,
                    Email = usuario.Email,
                    NombreCompleto = usuario.NombreCompleto,
                    Rol = usuario.Rol.ToString(),
                    EmpresaId = usuario.EmpresaId,
                    EmpresaNombre = usuario.Empresa?.Nombre
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en login");
            return StatusCode(500, new { error = "Error al procesar login", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Genera un token JWT para el usuario
    /// </summary>
    private string GenerarTokenJwt(TimeROD.Core.Entities.Usuario usuario)
    {
        var jwtKey = _configuration["Jwt:Key"];

        // En producción, leer de variable de entorno
        if (string.IsNullOrEmpty(jwtKey))
        {
            jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        }

        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT Key no configurada");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("empresaId", usuario.EmpresaId.ToString()),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
            new Claim("nombreCompleto", usuario.NombreCompleto)
        };

        var expiresInMinutes = _configuration.GetValue<int>("Jwt:ExpiresInMinutes", 480);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Endpoint de prueba para verificar si el token es válido
    /// </summary>
    [HttpGet("verify")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public ActionResult<object> VerifyToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                     User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value ??
                    User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
        var rol = User.FindFirst(ClaimTypes.Role)?.Value;
        var empresaId = User.FindFirst("empresaId")?.Value;

        return Ok(new
        {
            message = "Token válido",
            userId = userId,
            email = email,
            rol = rol,
            empresaId = empresaId
        });
    }
}

/// <summary>
/// Request para login
/// </summary>
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Response de login exitoso
/// </summary>
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UsuarioDto Usuario { get; set; } = new();
}

/// <summary>
/// DTO de usuario para respuesta de login
/// </summary>
public class UsuarioDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public int EmpresaId { get; set; }
    public string? EmpresaNombre { get; set; }
}
