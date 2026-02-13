using TimeROD.Core.DTOs;

namespace TimeROD.Core.Interfaces;

public interface IAreaService
{
    Task<IEnumerable<AreaDto>> GetAllAsync();
    Task<IEnumerable<AreaDto>> GetAllByEmpresaAsync(int empresaId);
    Task<AreaDto?> GetByIdAsync(int id);
    Task<AreaDto> CreateAsync(CreateAreaDto dto);
    Task UpdateAsync(int id, UpdateAreaDto dto);
    Task DeleteAsync(int id);
}
