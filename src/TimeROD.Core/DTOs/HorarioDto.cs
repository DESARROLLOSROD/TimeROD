using System.ComponentModel.DataAnnotations;

namespace TimeROD.Core.DTOs;

public class HorarioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string HoraEntrada { get; set; } = string.Empty; // HH:mm:ss
    public string HoraSalida { get; set; } = string.Empty;  // HH:mm:ss
    public int ToleranciaMinutos { get; set; }
    public bool Activo { get; set; }
}

public class CreateHorarioDto
{
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9](:[0-5][0-9])?$", ErrorMessage = "Formato de hora inválido (HH:mm:ss)")]
    public string HoraEntrada { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9](:[0-5][0-9])?$", ErrorMessage = "Formato de hora inválido (HH:mm:ss)")]
    public string HoraSalida { get; set; } = string.Empty;

    [Range(0, 120)]
    public int ToleranciaMinutos { get; set; }
}

public class UpdateHorarioDto : CreateHorarioDto
{
    public bool Activo { get; set; }
}
