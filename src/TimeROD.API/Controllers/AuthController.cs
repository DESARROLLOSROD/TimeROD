using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TimeROD.Core.DTOs;
using TimeROD.Core.Entities;
using TimeROD.Core.Interfaces;

namespace TimeROD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUsuarioService usuarioService,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _usuarioService = usuarioService;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Login de usuario
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            // Validar datos de entrada
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { error = "Email y password son requeridos" });
            }

            // Autenticar usando el servicio
            var usuario = await _usuarioService.AuthenticateAsync(request.Email, request.Password);

            if (usuario == null)
            {
                _logger.LogWarning("Login fallido para: {Email}", request.Email);
                return Unauthorized(new { error = "Email o password incorrectos" });
            }

            // Generar token JWT
            var token = GenerarTokenJwt(usuario);

            _logger.LogInformation("Login exitoso para usuario: {Email}", request.Email);

            return Ok(new LoginResponseDto
            {
                Token = token,
                Usuario = new UsuarioDto
                {
                    Id = usuario.Id,
                    Email = usuario.Email,
                    NombreCompleto = usuario.NombreCompleto,
                    Rol = usuario.Rol.ToString(),
                    EmpresaId = usuario.EmpresaId,
                    EmpresaNombre = usuario.Empresa?.Nombre,
                    Activo = usuario.Activo,
                    UltimoAcceso = usuario.UltimoAcceso
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
    private string GenerarTokenJwt(Usuario usuario)
    {
        // Priorizar variable de entorno (Producción)
        var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

        // Si no hay variable de entorno, usar configuración (Desarrollo)
        if (string.IsNullOrEmpty(jwtKey))
        {
            jwtKey = _configuration["Jwt:Key"];
        }

        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT Key no configurada (null or empty)");
        }
        
        // Debugging: Verificar longitud (log interno)
        if (jwtKey.Length < 16)
        {
            _logger.LogWarning("JWT Key es muy corta: {Length} caracteres", jwtKey.Length);
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
