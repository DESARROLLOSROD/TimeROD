using TimeROD.Core.DTOs;

namespace TimeROD.Core.Interfaces;

public interface IEmpleadoService
{
    Task<IEnumerable<EmpleadoDto>> GetAllAsync();
    Task<IEnumerable<EmpleadoDto>> GetAllByEmpresaAsync(int empresaId);
    Task<IEnumerable<EmpleadoDto>> GetAllByAreaAsync(int areaId);
    Task<EmpleadoDto?> GetByIdAsync(int id);
    Task<EmpleadoDto?> GetByNumeroEmpleadoAsync(string numeroEmpleado);
    Task<EmpleadoDto> CreateAsync(CreateEmpleadoDto dto);
    Task UpdateAsync(int id, UpdateEmpleadoDto dto);
    Task DeleteAsync(int id);
}
