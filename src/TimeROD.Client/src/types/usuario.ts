export interface UsuarioDto {
    id: number;
    nombreCompleto: string;
    email: string;
    rol: string; // 'Admin', 'Supervisor', 'Empleado', 'RH'
    empresaId: number;
    empresaNombre?: string;
    activo: boolean;
    ultimoAcceso?: string;
}

export interface CreateUsuarioDto {
    nombreCompleto: string;
    email: string;
    password?: string; // Optional if auto-generated or handled via invite
    rol: string;
    empresaId: number;
    activo: boolean;
}

export interface UpdateUsuarioDto {
    nombreCompleto: string;
    email: string;
    rol: string;
    empresaId: number;
    activo: boolean;
    password?: string; // Optional for password update
}
