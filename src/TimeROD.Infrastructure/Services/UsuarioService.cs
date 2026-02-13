using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TimeROD.Core.DTOs;
using TimeROD.Core.Entities;
using TimeROD.Core.Interfaces;
using TimeROD.Infrastructure.Data;

namespace TimeROD.Infrastructure.Services;

public class UsuarioService : IUsuarioService
{
    private readonly TimeRODDbContext _context;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(TimeRODDbContext context, ILogger<UsuarioService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<UsuarioDto>> GetAllAsync(int? empresaId = null)
    {
        var query = _context.Usuarios
            .Include(u => u.Empresa)
            .Where(u => u.Activo)
            .AsQueryable();

        if (empresaId.HasValue)
        {
            query = query.Where(u => u.EmpresaId == empresaId.Value);
        }

        var usuarios = await query
            .OrderBy(u => u.NombreCompleto)
            .ToListAsync();

        return usuarios.Select(MapToDto);
    }

    public async Task<UsuarioDto?> GetByIdAsync(int id)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Empresa)
            .FirstOrDefaultAsync(u => u.Id == id);

        return usuario == null ? null : MapToDto(usuario);
    }

    public async Task<UsuarioDto> CreateAsync(CreateUsuarioDto dto)
    {
        // Validar email único
        var emailExiste = await _context.Usuarios
            .AnyAsync(u => u.Email == dto.Email);

        if (emailExiste)
        {
            throw new InvalidOperationException($"El email {dto.Email} ya está registrado");
        }

        var usuario = new Usuario
        {
            Email = dto.Email,
            PasswordHash = Base64Encode(dto.Password), // Temporalmente Base64, luego BCrypt en refactor
            NombreCompleto = dto.NombreCompleto,
            Rol = Enum.Parse<RolUsuario>(dto.Rol),
            EmpresaId = dto.EmpresaId,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        // TODO: Usar hasher real inyectado o BCrypt directamente aquí si ya tenemos la dependencia
        // Por ahora usamos la lógica que estaba en el controller (BCrypt directo)
        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        // Recargar con relaciones si es necesario
        await _context.Entry(usuario).Reference(u => u.Empresa).LoadAsync();

        return MapToDto(usuario);
    }

    public async Task UpdateAsync(int id, UpdateUsuarioDto dto)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            throw new KeyNotFoundException($"Usuario con ID {id} no encontrado");

        // Validar email único (excluyendo el actual)
        var emailExiste = await _context.Usuarios
            .AnyAsync(u => u.Email == dto.Email && u.Id != id);

        if (emailExiste)
            throw new InvalidOperationException($"El email {dto.Email} ya está registrado por otro usuario");

        usuario.Email = dto.Email;
        usuario.NombreCompleto = dto.NombreCompleto;
        usuario.Rol = Enum.Parse<RolUsuario>(dto.Rol);
        usuario.EmpresaId = dto.EmpresaId;
        usuario.Activo = dto.Activo;

        if (!string.IsNullOrEmpty(dto.Password))
        {
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            throw new KeyNotFoundException($"Usuario con ID {id} no encontrado");

        usuario.Activo = false; // Soft Delete
        await _context.SaveChangesAsync();
    }

    public async Task<Usuario?> AuthenticateAsync(string email, string password)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Empresa)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        if (usuario == null || !usuario.Activo)
            return null;

        // Verificar password
        bool passwordValido = false;
        bool needsRehash = false;

        try
        {
            passwordValido = BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);
        }
        catch (Exception)
        {
            // Legacy plain text check
            if (usuario.PasswordHash == password)
            {
                passwordValido = true;
                needsRehash = true;
            }
        }

        if (!passwordValido)
            return null;

        if (needsRehash)
        {
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            await _context.SaveChangesAsync();
        }

        usuario.UltimoAcceso = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return usuario;
    }

    private static UsuarioDto MapToDto(Usuario u)
    {
        return new UsuarioDto
        {
            Id = u.Id,
            Email = u.Email,
            NombreCompleto = u.NombreCompleto,
            Rol = u.Rol.ToString(),
            Activo = u.Activo,
            EmpresaId = u.EmpresaId,
            EmpresaNombre = u.Empresa?.Nombre,
            UltimoAcceso = u.UltimoAcceso
        };
    }

    private string Base64Encode(string plainText) 
    {
        // Placeholder helper, not really used since we use BCrypt
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }
}
