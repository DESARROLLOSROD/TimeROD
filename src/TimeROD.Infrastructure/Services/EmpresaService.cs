using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TimeROD.Core.DTOs;
using TimeROD.Core.Entities;
using TimeROD.Core.Interfaces;
using TimeROD.Infrastructure.Data;

namespace TimeROD.Infrastructure.Services;

public class EmpresaService : IEmpresaService
{
    private readonly TimeRODDbContext _context;
    private readonly ILogger<EmpresaService> _logger;

    public EmpresaService(TimeRODDbContext context, ILogger<EmpresaService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<EmpresaDto>> GetAllAsync()
    {
        var empresas = await _context.Empresas
            .Where(e => e.Activa)
            .OrderBy(e => e.Nombre)
            .ToListAsync();

        return empresas.Select(MapToDto);
    }

    public async Task<EmpresaDto?> GetByIdAsync(int id)
    {
        var empresa = await _context.Empresas.FindAsync(id);
        return empresa == null ? null : MapToDto(empresa);
    }

    public async Task<EmpresaDto> CreateAsync(CreateEmpresaDto dto)
    {
        // Validar RFC único
        var rfcExiste = await _context.Empresas
            .AnyAsync(e => e.RFC == dto.RFC);

        if (rfcExiste)
        {
            throw new InvalidOperationException($"El RFC {dto.RFC} ya está registrado");
        }

        var empresa = new Empresa
        {
            Nombre = dto.Nombre,
            RFC = dto.RFC,
            Direccion = dto.Direccion,
            ConfiguracionJson = dto.ConfiguracionJson,
            Activa = true,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Empresas.Add(empresa);
        await _context.SaveChangesAsync();

        return MapToDto(empresa);
    }

    public async Task UpdateAsync(int id, UpdateEmpresaDto dto)
    {
        var empresa = await _context.Empresas.FindAsync(id);

        if (empresa == null)
        {
            throw new KeyNotFoundException($"Empresa con ID {id} no encontrada");
        }

        // Validar RFC único (excepto la misma empresa)
        var rfcExiste = await _context.Empresas
            .AnyAsync(e => e.RFC == dto.RFC && e.Id != id);

        if (rfcExiste)
        {
            throw new InvalidOperationException($"El RFC {dto.RFC} ya está registrado por otra empresa");
        }

        empresa.Nombre = dto.Nombre;
        empresa.RFC = dto.RFC;
        empresa.Direccion = dto.Direccion;
        empresa.ConfiguracionJson = dto.ConfiguracionJson;
        // Permite reactivar o desactivar desde update si se desea, aunque Delete es lo ideal para desactivar
        empresa.Activa = dto.Activa; 
        empresa.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var empresa = await _context.Empresas
            .Include(e => e.Usuarios)
            .Include(e => e.Areas)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (empresa == null)
        {
            throw new KeyNotFoundException($"Empresa con ID {id} no encontrada");
        }

        // Validar dependencias activas
        var usuariosActivos = empresa.Usuarios?.Count(u => u.Activo) ?? 0;
        var areasActivas = empresa.Areas?.Count(a => a.Activa) ?? 0;

        if (usuariosActivos > 0 || areasActivas > 0)
        {
            throw new InvalidOperationException($"No se puede desactivar la empresa porque tiene {usuariosActivos} usuarios activos o {areasActivas} áreas activas.");
        }

        // Soft delete
        empresa.Activa = false;
        empresa.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    private static EmpresaDto MapToDto(Empresa e)
    {
        return new EmpresaDto
        {
            Id = e.Id,
            Nombre = e.Nombre,
            RFC = e.RFC,
            Direccion = e.Direccion,
            ConfiguracionJson = e.ConfiguracionJson,
            Activa = e.Activa
        };
    }
}
