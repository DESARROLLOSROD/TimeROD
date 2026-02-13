export interface EmpresaDto {
    id: number;
    nombre: string;
    rfc: string;
    direccion: string;
    telefono: string;
    activo: boolean;
    createdAt: string;
}

export interface CreateEmpresaDto {
    nombre: string;
    rfc: string;
    direccion?: string;
    telefono?: string;
    activo: boolean;
}

export interface UpdateEmpresaDto {
    nombre: string;
    rfc: string;
    direccion?: string;
    telefono?: string;
    activo: boolean;
}
