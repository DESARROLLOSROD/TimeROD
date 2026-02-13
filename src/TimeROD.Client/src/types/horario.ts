export interface HorarioDto {
    id: number;
    nombre: string;
    horaEntrada: string; // HH:mm:ss
    horaSalida: string; // HH:mm:ss
    toleranciaMinutos: number;
    activo: boolean;
}

export interface CreateHorarioDto {
    nombre: string;
    horaEntrada: string;
    horaSalida: string;
    toleranciaMinutos: number;
}

export interface UpdateHorarioDto extends CreateHorarioDto {
    activo: boolean;
}
