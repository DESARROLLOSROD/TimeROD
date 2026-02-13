using TimeROD.Core.DTOs;

namespace TimeROD.Core.Interfaces;

public interface IHorarioService
{
    Task<IEnumerable<HorarioDto>> GetAllAsync();
    Task<HorarioDto?> GetByIdAsync(int id);
    Task<HorarioDto> CreateAsync(CreateHorarioDto dto);
    Task UpdateAsync(int id, UpdateHorarioDto dto);
    Task DeleteAsync(int id);
}
