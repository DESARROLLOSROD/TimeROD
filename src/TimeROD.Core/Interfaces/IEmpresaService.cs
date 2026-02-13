using TimeROD.Core.DTOs;

namespace TimeROD.Core.Interfaces;

public interface IEmpresaService
{
    Task<IEnumerable<EmpresaDto>> GetAllAsync();
    Task<EmpresaDto?> GetByIdAsync(int id);
    Task<EmpresaDto> CreateAsync(CreateEmpresaDto dto);
    Task UpdateAsync(int id, UpdateEmpresaDto dto);
    Task DeleteAsync(int id);
}
