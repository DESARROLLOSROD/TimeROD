export interface AsistenciaDto {
    id: number;
    empleadoId: number;
    empleadoNombreCompleto?: string;
    empleadoNumero?: string;
    empresaId: number;
    empresaNombre?: string;
    areaId: number;
    areaNombre?: string;
    fecha: string; // ISO Date (YYYY-MM-DD)
    horaEntrada: string; // HH:mm:ss
    horaSalida?: string; // HH:mm:ss
    tipo: string; // 'Normal', 'Retardo', 'Falta', etc.
    observaciones?: string;
    llegadaTardia: boolean;
    minutosRetraso: number | null;
    salidaAnticipada: boolean;
    minutosAnticipados: number | null;
    horasTrabajadas?: number;
}

export interface RegistroEntradaDto {
    empleadoId: number;
    fecha: string; // ISO Date
    hora: string; // HH:mm:ss
    latitud?: number;
    longitud?: number;
}

export interface RegistroSalidaDto {
    empleadoId: number;
    fecha: string; // ISO Date
    hora: string; // HH:mm:ss
    latitud?: number;
    longitud?: number;
}

export interface UpdateAsistenciaDto {
    horaEntrada?: string;
    horaSalida?: string;
    tipo: string;
    notas?: string;
    aprobado: boolean;
    llegadaTardia: boolean;
    minutosRetraso?: number;
    salidaAnticipada: boolean;
    minutosAnticipados?: number;
}
