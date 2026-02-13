export interface EmpleadoDto {
    id: number;
    empresaId: number;
    empresaNombre?: string;
    areaId: number;
    areaNombre?: string;
    usuarioId?: number;
    usuarioNombre?: string;
    numeroEmpleado: string;
    nombre: string;
    apellidos: string;
    fechaIngreso: string; // ISO Date
    salarioDiario: number;
    turnoId?: number;
    idBiometrico?: number;
    activo: boolean;
    puesto?: string;
}

export interface CreateEmpleadoDto {
    empresaId: number;
    areaId: number;
    usuarioId?: number;
    numeroEmpleado: string;
    nombre: string;
    apellidos: string;
    fechaIngreso: string;
    salarioDiario: number;
    turnoId?: number;
    idBiometrico?: number;
    activo: boolean;
    puesto?: string;
}

export interface UpdateEmpleadoDto {
    empresaId: number;
    areaId: number;
    usuarioId?: number;
    numeroEmpleado: string;
    nombre: string;
    apellidos: string;
    fechaIngreso: string;
    salarioDiario: number;
    turnoId?: number;
    idBiometrico?: number;
    activo: boolean;
    puesto?: string;
}
