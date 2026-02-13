export interface LoginRequestDto {
    email: string;
    password?: string;
}

export interface LoginResponseDto {
    token: string;
    usuario: UsuarioDto;
}

export interface UsuarioDto {
    id: number;
    email: string;
    nombreCompleto: string;
    rol: string;
    empresaId: number;
    empresaNombre?: string;
    activo: boolean;
    ultimoAcceso?: string;
}
