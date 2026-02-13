import api from './api';
import type { EmpresaDto, CreateEmpresaDto, UpdateEmpresaDto } from '../types/empresa';

const empresaService = {
    getAll: async (): Promise<EmpresaDto[]> => {
        const response = await api.get<EmpresaDto[]>('/empresas');
        return response.data;
    },

    getById: async (id: number): Promise<EmpresaDto> => {
        const response = await api.get<EmpresaDto>(`/empresas/${id}`);
        return response.data;
    },

    create: async (empresa: CreateEmpresaDto): Promise<EmpresaDto> => {
        const response = await api.post<EmpresaDto>('/empresas', empresa);
        return response.data;
    },

    update: async (id: number, empresa: UpdateEmpresaDto): Promise<void> => {
        await api.put(`/empresas/${id}`, empresa);
    },

    delete: async (id: number): Promise<void> => {
        await api.delete(`/empresas/${id}`);
    }
};

export default empresaService;
