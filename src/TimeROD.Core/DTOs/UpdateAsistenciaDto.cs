using System.ComponentModel.DataAnnotations;
using TimeROD.Core.Entities;

namespace TimeROD.Core.DTOs;

public class UpdateAsistenciaDto
{
    public DateTime? HoraEntrada { get; set; }
    public DateTime? HoraSalida { get; set; }
    public TipoAsistencia Tipo { get; set; }
    public string? Notas { get; set; }
    public bool Aprobado { get; set; }
    public bool LlegadaTardia { get; set; }
    public int? MinutosRetraso { get; set; }
}
