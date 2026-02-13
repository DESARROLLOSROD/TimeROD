using TimeROD.Core.DTOs;
using TimeROD.Core.Entities;

namespace TimeROD.Core.Interfaces;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDto>> GetAllAsync(int? empresaId = null);
    Task<UsuarioDto?> GetByIdAsync(int id);
    Task<UsuarioDto> CreateAsync(CreateUsuarioDto dto);
    Task UpdateAsync(int id, UpdateUsuarioDto dto);
    Task DeleteAsync(int id);
    Task<Usuario?> AuthenticateAsync(string email, string password);
}
