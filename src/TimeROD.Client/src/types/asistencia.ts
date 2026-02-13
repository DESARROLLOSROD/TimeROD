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
    fecha: string;
    horaEntrada: string;
    horaSalida?: string;
    tipo: string;
    observaciones?: string;
}
