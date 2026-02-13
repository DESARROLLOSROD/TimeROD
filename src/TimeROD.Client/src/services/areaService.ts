import api from './api';
import type { AreaDto, CreateAreaDto, UpdateAreaDto } from '../types/area';

const areaService = {
    getAll: async (): Promise<AreaDto[]> => {
        const response = await api.get<AreaDto[]>('/areas');
        return response.data;
    },

    getAllByEmpresa: async (empresaId: number): Promise<AreaDto[]> => {
        const response = await api.get<AreaDto[]>(`/areas/empresa/${empresaId}`);
        return response.data;
    },

    getById: async (id: number): Promise<AreaDto> => {
        const response = await api.get<AreaDto>(`/areas/${id}`);
        return response.data;
    },

    create: async (area: CreateAreaDto): Promise<AreaDto> => {
        const response = await api.post<AreaDto>('/areas', area);
        return response.data;
    },

    update: async (id: number, area: UpdateAreaDto): Promise<void> => {
        await api.put(`/areas/${id}`, area);
    },

    delete: async (id: number): Promise<void> => {
        await api.delete(`/areas/${id}`);
    }
};

export default areaService;
