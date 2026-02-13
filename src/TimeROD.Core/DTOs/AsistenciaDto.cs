using TimeROD.Core.Entities;

namespace TimeROD.Core.DTOs;

public class AsistenciaDto
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public string? EmpleadoNombreCompleto { get; set; }
    public string? EmpleadoNumero { get; set; }
    public int EmpresaId { get; set; }
    public string? EmpresaNombre { get; set; }
    public int AreaId { get; set; }
    public string? AreaNombre { get; set; }
    public DateTime Fecha { get; set; }
    public DateTime? HoraEntrada { get; set; }
    public DateTime? HoraSalida { get; set; }
    public TipoAsistencia Tipo { get; set; }
    public string? Notas { get; set; }
    public bool Aprobado { get; set; }
    public decimal? HorasTrabajadas { get; set; }
    public bool LlegadaTardia { get; set; }
    public int? MinutosRetraso { get; set; }
}
