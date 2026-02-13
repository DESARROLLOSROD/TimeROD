import api from './api';
import type { UsuarioDto, CreateUsuarioDto, UpdateUsuarioDto } from '../types/usuario';

const usuarioService = {
    getAll: async (): Promise<UsuarioDto[]> => {
        const response = await api.get<UsuarioDto[]>('/usuarios');
        return response.data;
    },

    getAllByEmpresa: async (empresaId: number): Promise<UsuarioDto[]> => {
        const response = await api.get<UsuarioDto[]>(`/usuarios/empresa/${empresaId}`);
        return response.data;
    },

    getById: async (id: number): Promise<UsuarioDto> => {
        const response = await api.get<UsuarioDto>(`/usuarios/${id}`);
        return response.data;
    },

    create: async (usuario: CreateUsuarioDto): Promise<UsuarioDto> => {
        const response = await api.post<UsuarioDto>('/usuarios', usuario);
        return response.data;
    },

    update: async (id: number, usuario: UpdateUsuarioDto): Promise<void> => {
        await api.put(`/usuarios/${id}`, usuario);
    },

    delete: async (id: number): Promise<void> => {
        await api.delete(`/usuarios/${id}`);
    }
};

export default usuarioService;
