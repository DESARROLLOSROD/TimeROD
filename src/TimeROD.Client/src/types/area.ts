export interface AreaDto {
    id: number;
    nombre: string;
    descripcion?: string;
    empresaId: number;
    empresaNombre?: string;
    activo: boolean;
    horarioId?: number;
}

export interface CreateAreaDto {
    nombre: string;
    descripcion?: string;
    empresaId: number;
    activo: boolean;
    horarioId?: number;
}

export interface UpdateAreaDto {
    nombre: string;
    descripcion?: string;
    empresaId: number;
    activo: boolean;
    horarioId?: number;
}
