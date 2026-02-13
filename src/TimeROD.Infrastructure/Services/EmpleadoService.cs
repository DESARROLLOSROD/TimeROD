using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TimeROD.Core.DTOs;
using TimeROD.Core.Entities;
using TimeROD.Core.Interfaces;
using TimeROD.Infrastructure.Data;

namespace TimeROD.Infrastructure.Services;

public class EmpleadoService : IEmpleadoService
{
    private readonly TimeRODDbContext _context;
    private readonly ILogger<EmpleadoService> _logger;

    public EmpleadoService(TimeRODDbContext context, ILogger<EmpleadoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<EmpleadoDto>> GetAllAsync()
    {
        var empleados = await _context.Empleados
            .Include(e => e.Empresa)
            .Include(e => e.Area)
            .Include(e => e.Usuario)
            .Where(e => e.Activo)
            .OrderBy(e => e.Apellidos)
            .ThenBy(e => e.Nombre)
            .ToListAsync();

        return empleados.Select(e => MapToDto(e));
    }

    public async Task<IEnumerable<EmpleadoDto>> GetAllByEmpresaAsync(int empresaId)
    {
        var empleados = await _context.Empleados
            .Include(e => e.Area)
            .Include(e => e.Usuario)
            .Where(e => e.EmpresaId == empresaId && e.Activo)
            .OrderBy(e => e.Apellidos)
            .ThenBy(e => e.Nombre)
            .ToListAsync();

        return empleados.Select(e => MapToDto(e));
    }

    public async Task<IEnumerable<EmpleadoDto>> GetAllByAreaAsync(int areaId)
    {
        var empleados = await _context.Empleados
            .Include(e => e.Usuario)
            .Include(e => e.Empresa)
            .Where(e => e.AreaId == areaId && e.Activo)
            .OrderBy(e => e.Apellidos)
            .ThenBy(e => e.Nombre)
            .ToListAsync();

        return empleados.Select(e => MapToDto(e));
    }

    public async Task<EmpleadoDto?> GetByIdAsync(int id)
    {
        var empleado = await _context.Empleados
            .Include(e => e.Empresa)
            .Include(e => e.Area)
            .Include(e => e.Usuario)
            .FirstOrDefaultAsync(e => e.Id == id);

        return empleado == null ? null : MapToDto(empleado);
    }
    
    public async Task<EmpleadoDto?> GetByNumeroEmpleadoAsync(string numeroEmpleado)
    {
         var empleado = await _context.Empleados
            .Include(e => e.Empresa)
            .Include(e => e.Area)
            .Include(e => e.Usuario)
            .FirstOrDefaultAsync(e => e.NumeroEmpleado == numeroEmpleado);

        return empleado == null ? null : MapToDto(empleado);
    }

    public async Task<EmpleadoDto> CreateAsync(CreateEmpleadoDto dto)
    {
        // 1. Validar Empresa
        var empresaExiste = await _context.Empresas.AnyAsync(e => e.Id == dto.EmpresaId);
        if (!empresaExiste)
        {
            throw new InvalidOperationException($"Empresa con ID {dto.EmpresaId} no encontrada");
        }

        // 2. Validar Area (y que pertenezca a la empresa)
        var areaExiste = await _context.Areas
            .AnyAsync(a => a.Id == dto.AreaId && a.EmpresaId == dto.EmpresaId);
        if (!areaExiste)
        {
            throw new InvalidOperationException($"Área con ID {dto.AreaId} no encontrada o no pertenece a la empresa");
        }

        // 3. Validar Usuario (y que pertenezca a la empresa)
        if (dto.UsuarioId.HasValue)
        {
            var usuarioExiste = await _context.Usuarios
                .AnyAsync(u => u.Id == dto.UsuarioId.Value && u.EmpresaId == dto.EmpresaId);
            if (!usuarioExiste)
            {
                throw new InvalidOperationException($"Usuario con ID {dto.UsuarioId} no encontrado o no pertenece a la empresa");
            }
        }

        // 4. Validar NumeroEmpleado único en la empresa
        var numeroExiste = await _context.Empleados
            .AnyAsync(e => e.NumeroEmpleado == dto.NumeroEmpleado && e.EmpresaId == dto.EmpresaId);
        if (numeroExiste)
        {
             throw new InvalidOperationException($"Número de empleado {dto.NumeroEmpleado} ya existe en esta empresa");
        }

        var empleado = new Empleado
        {
            EmpresaId = dto.EmpresaId,
            AreaId = dto.AreaId,
            UsuarioId = dto.UsuarioId,
            NumeroEmpleado = dto.NumeroEmpleado,
            Nombre = dto.Nombre,
            Apellidos = dto.Apellidos,
            FechaIngreso = dto.FechaIngreso,
            SalarioDiario = dto.SalarioDiario,
            TurnoId = dto.TurnoId,
            IdBiometrico = dto.IdBiometrico,
            Puesto = dto.Puesto,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Empleados.Add(empleado);
        await _context.SaveChangesAsync();

        // Cargar relaciones para el DTO
        await _context.Entry(empleado).Reference(e => e.Empresa).LoadAsync();
        await _context.Entry(empleado).Reference(e => e.Area).LoadAsync();
        if (empleado.UsuarioId.HasValue)
            await _context.Entry(empleado).Reference(e => e.Usuario).LoadAsync();

        return MapToDto(empleado);
    }

    public async Task UpdateAsync(int id, UpdateEmpleadoDto dto)
    {
        var empleado = await _context.Empleados.FindAsync(id);
        if (empleado == null)
        {
             throw new KeyNotFoundException($"Empleado con ID {id} no encontrado");
        }

        // 1. Validar Empresa (si cambió)
        if (empleado.EmpresaId != dto.EmpresaId)
        {
             var empresaExiste = await _context.Empresas.AnyAsync(e => e.Id == dto.EmpresaId);
            if (!empresaExiste) throw new InvalidOperationException($"Empresa con ID {dto.EmpresaId} no encontrada");
        }

        // 2. Validar Area
        if (empleado.AreaId != dto.AreaId || empleado.EmpresaId != dto.EmpresaId)
        {
             var areaExiste = await _context.Areas
                .AnyAsync(a => a.Id == dto.AreaId && a.EmpresaId == dto.EmpresaId);
            if (!areaExiste) throw new InvalidOperationException($"Área con ID {dto.AreaId} no encontrada o no pertenece a la empresa");
        }

        // 3. Validar Usuario
        if (dto.UsuarioId.HasValue && (dto.UsuarioId != empleado.UsuarioId || empleado.EmpresaId != dto.EmpresaId))
        {
             var usuarioExiste = await _context.Usuarios
                .AnyAsync(u => u.Id == dto.UsuarioId.Value && u.EmpresaId == dto.EmpresaId);
            if (!usuarioExiste) throw new InvalidOperationException($"Usuario con ID {dto.UsuarioId} no encontrado o no pertenece a la empresa");
        }

        // 4. Validar NumeroEmpleado único
        var numeroExiste = await _context.Empleados
            .AnyAsync(e => e.NumeroEmpleado == dto.NumeroEmpleado &&
                           e.EmpresaId == dto.EmpresaId &&
                           e.Id != id);
        if (numeroExiste)
        {
             throw new InvalidOperationException($"Número de empleado {dto.NumeroEmpleado} ya existe en esta empresa");
        }

        empleado.EmpresaId = dto.EmpresaId;
        empleado.AreaId = dto.AreaId;
        empleado.UsuarioId = dto.UsuarioId;
        empleado.NumeroEmpleado = dto.NumeroEmpleado;
        empleado.Nombre = dto.Nombre;
        empleado.Apellidos = dto.Apellidos;
        empleado.FechaIngreso = dto.FechaIngreso;
        empleado.SalarioDiario = dto.SalarioDiario;
        empleado.TurnoId = dto.TurnoId;
        empleado.IdBiometrico = dto.IdBiometrico;
        empleado.Puesto = dto.Puesto;
        empleado.Activo = dto.Activo;
        empleado.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
         var empleado = await _context.Empleados.FindAsync(id);
        if (empleado == null)
        {
             throw new KeyNotFoundException($"Empleado con ID {id} no encontrado");
        }

        // Soft delete
        empleado.Activo = false;
        empleado.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    private static EmpleadoDto MapToDto(Empleado e, string? empresaNombre = null)
    {
        return new EmpleadoDto
        {
            Id = e.Id,
            EmpresaId = e.EmpresaId,
            EmpresaNombre = empresaNombre ?? e.Empresa?.Nombre,
            AreaId = e.AreaId,
            AreaNombre = e.Area?.Nombre,
            UsuarioId = e.UsuarioId,
            UsuarioNombre = e.Usuario?.NombreCompleto,
            NumeroEmpleado = e.NumeroEmpleado,
            Nombre = e.Nombre,
            Apellidos = e.Apellidos,
            FechaIngreso = e.FechaIngreso,
            SalarioDiario = e.SalarioDiario,
            TurnoId = e.TurnoId,
            IdBiometrico = e.IdBiometrico,
            Activo = e.Activo,
            Puesto = e.Puesto
        };
    }
}
