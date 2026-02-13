using TimeROD.Core.DTOs;

namespace TimeROD.Core.Interfaces;

public interface IAsistenciaService
{
    Task<IEnumerable<AsistenciaDto>> GetAllAsync(int? empleadoId = null, DateTime? fechaInicio = null, DateTime? fechaFin = null);
    Task<AsistenciaDto?> GetByIdAsync(int id);
    Task<IEnumerable<AsistenciaDto>> GetByEmpleadoAsync(int empleadoId, DateTime? fechaInicio = null, DateTime? fechaFin = null);
    Task<AsistenciaDto> RegistrarEntradaAsync(RegistroEntradaDto dto);
    Task<AsistenciaDto> RegistrarSalidaAsync(RegistroSalidaDto dto);
    Task UpdateAsync(int id, UpdateAsistenciaDto dto);
    Task DeleteAsync(int id);
    Task<object> GetReporteAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null, int? empresaId = null);
}
