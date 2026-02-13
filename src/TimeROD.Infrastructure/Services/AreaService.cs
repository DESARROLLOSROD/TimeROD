using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TimeROD.Core.DTOs;
using TimeROD.Core.Entities;
using TimeROD.Core.Interfaces;
using TimeROD.Infrastructure.Data;

namespace TimeROD.Infrastructure.Services;

public class AreaService : IAreaService
{
    private readonly TimeRODDbContext _context;
    private readonly ILogger<AreaService> _logger;

    public AreaService(TimeRODDbContext context, ILogger<AreaService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<AreaDto>> GetAllAsync()
    {
        var areas = await _context.Areas
            .Include(a => a.Empresa)
            .Include(a => a.Supervisor)
            .Where(a => a.Activa)
            .OrderBy(a => a.Nombre)
            .ToListAsync();

        return areas.Select(MapToDto);
    }

    public async Task<IEnumerable<AreaDto>> GetAllByEmpresaAsync(int empresaId)
    {
         var areas = await _context.Areas
            .Include(a => a.Empresa)
            .Include(a => a.Supervisor)
            .Where(a => a.EmpresaId == empresaId && a.Activa)
            .OrderBy(a => a.Nombre)
            .ToListAsync();

        return areas.Select(MapToDto);
    }

    public async Task<AreaDto?> GetByIdAsync(int id)
    {
        var area = await _context.Areas
            .Include(a => a.Empresa)
            .Include(a => a.Supervisor)
            .FirstOrDefaultAsync(a => a.Id == id);

        return area == null ? null : MapToDto(area);
    }

    public async Task<AreaDto> CreateAsync(CreateAreaDto dto)
    {
        // Validar Empresa
        var empresaExiste = await _context.Empresas.AnyAsync(e => e.Id == dto.EmpresaId);
        if (!empresaExiste)
        {
            throw new InvalidOperationException($"Empresa con ID {dto.EmpresaId} no encontrada");
        }

        // Validar Supervisor (si aplica)
        if (dto.SupervisorId.HasValue)
        {
            var supervisorExiste = await _context.Usuarios
                .AnyAsync(u => u.Id == dto.SupervisorId.Value && u.Activo);

            if (!supervisorExiste)
            {
                throw new InvalidOperationException($"Supervisor con ID {dto.SupervisorId} no encontrado o inactivo");
            }
        }

        var area = new Area
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            EmpresaId = dto.EmpresaId,
            SupervisorId = dto.SupervisorId,
            Activa = true,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Areas.Add(area);
        await _context.SaveChangesAsync();

        // Recargar relaciones para el DTO
        await _context.Entry(area).Reference(a => a.Empresa).LoadAsync();
        if (area.SupervisorId.HasValue)
            await _context.Entry(area).Reference(a => a.Supervisor).LoadAsync();

        return MapToDto(area);
    }

    public async Task UpdateAsync(int id, UpdateAreaDto dto)
    {
        var area = await _context.Areas.FindAsync(id);
        if (area == null)
        {
            throw new KeyNotFoundException($"Área con ID {id} no encontrada");
        }

        // Validar Empresa (si cambió, aunque el DTO pide EmpresaId required, asumimos que puede cambiar)
        if (area.EmpresaId != dto.EmpresaId)
        {
             var empresaExiste = await _context.Empresas.AnyAsync(e => e.Id == dto.EmpresaId);
            if (!empresaExiste)
            {
                throw new InvalidOperationException($"Empresa con ID {dto.EmpresaId} no encontrada");
            }
        }

        // Validar Supervisor
        if (dto.SupervisorId.HasValue && dto.SupervisorId != area.SupervisorId)
        {
            var supervisorExiste = await _context.Usuarios
                .AnyAsync(u => u.Id == dto.SupervisorId.Value && u.Activo);

            if (!supervisorExiste)
            {
                throw new InvalidOperationException($"Supervisor con ID {dto.SupervisorId} no encontrado o inactivo");
            }
        }

        area.Nombre = dto.Nombre;
        area.Descripcion = dto.Descripcion;
        area.EmpresaId = dto.EmpresaId;
        area.SupervisorId = dto.SupervisorId;
        area.Activa = dto.Activa;
        area.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var area = await _context.Areas
            .Include(a => a.Empleados)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (area == null)
        {
            throw new KeyNotFoundException($"Área con ID {id} no encontrada");
        }

        // Validar dependencias (Empleados activos)
        var empleadosActivos = area.Empleados?.Count(e => e.Activo) ?? 0;
        if (empleadosActivos > 0)
        {
            throw new InvalidOperationException($"No se puede desactivar el área porque tiene {empleadosActivos} empleado(s) activo(s)");
        }

        area.Activa = false;
        area.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    private static AreaDto MapToDto(Area a)
    {
        return new AreaDto
        {
            Id = a.Id,
            Nombre = a.Nombre,
            Descripcion = a.Descripcion,
            Activa = a.Activa,
            EmpresaId = a.EmpresaId,
            EmpresaNombre = a.Empresa?.Nombre,
            SupervisorId = a.SupervisorId,
            SupervisorNombre = a.Supervisor?.NombreCompleto
        };
    }
}
