using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TimeROD.Core.DTOs;
using TimeROD.Core.Entities;
using TimeROD.Core.Interfaces;
using TimeROD.Infrastructure.Data;

namespace TimeROD.Infrastructure.Services;

public class HorarioService : IHorarioService
{
    private readonly TimeRODDbContext _context;
    private readonly ILogger<HorarioService> _logger;

    public HorarioService(TimeRODDbContext context, ILogger<HorarioService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<HorarioDto>> GetAllAsync()
    {
        var horarios = await _context.Horarios
            .Where(h => h.Activo)
            .OrderBy(h => h.Nombre)
            .ToListAsync();

        return horarios.Select(MapToDto);
    }

    public async Task<HorarioDto?> GetByIdAsync(int id)
    {
        var horario = await _context.Horarios.FindAsync(id);
        return horario == null ? null : MapToDto(horario);
    }

    public async Task<HorarioDto> CreateAsync(CreateHorarioDto dto)
    {
        var horario = new Horario
        {
            Nombre = dto.Nombre,
            HoraEntrada = TimeSpan.Parse(dto.HoraEntrada),
            HoraSalida = TimeSpan.Parse(dto.HoraSalida),
            ToleranciaMinutos = dto.ToleranciaMinutos,
            Activo = true
        };

        _context.Horarios.Add(horario);
        await _context.SaveChangesAsync();

        return MapToDto(horario);
    }

    public async Task UpdateAsync(int id, UpdateHorarioDto dto)
    {
        var horario = await _context.Horarios.FindAsync(id);

        if (horario == null)
        {
            throw new KeyNotFoundException($"Horario con ID {id} no encontrado");
        }

        horario.Nombre = dto.Nombre;
        horario.HoraEntrada = TimeSpan.Parse(dto.HoraEntrada);
        horario.HoraSalida = TimeSpan.Parse(dto.HoraSalida);
        horario.ToleranciaMinutos = dto.ToleranciaMinutos;
        horario.Activo = dto.Activo;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var horario = await _context.Horarios.FindAsync(id);
        if (horario == null)
        {
            throw new KeyNotFoundException($"Horario con ID {id} no encontrado");
        }

        // Soft delete
        horario.Activo = false;
        await _context.SaveChangesAsync();
    }

    private static HorarioDto MapToDto(Horario h)
    {
        return new HorarioDto
        {
            Id = h.Id,
            Nombre = h.Nombre,
            HoraEntrada = h.HoraEntrada.ToString(@"hh\:mm\:ss"),
            HoraSalida = h.HoraSalida.ToString(@"hh\:mm\:ss"),
            ToleranciaMinutos = h.ToleranciaMinutos,
            Activo = h.Activo
        };
    }
}
