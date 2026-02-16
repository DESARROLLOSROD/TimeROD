import api from './api';
import type { HorarioDto, CreateHorarioDto, UpdateHorarioDto } from '../types/horario';

const horarioService = {
    getAll: async (): Promise<HorarioDto[]> => {
        const response = await api.get<HorarioDto[]>('/horarios');
        return response.data;
    },

    getById: async (id: number): Promise<HorarioDto> => {
        const response = await api.get<HorarioDto>(`/horarios/${id}`);
        return response.data;
    },

    create: async (horario: CreateHorarioDto): Promise<HorarioDto> => {
        const response = await api.post<HorarioDto>('/horarios', horario);
        return response.data;
    },

    update: async (id: number, horario: UpdateHorarioDto): Promise<void> => {
        const response = await api.put(`/horarios/${id}`, horario);
        return response.data;
    },

    delete: async (id: number): Promise<void> => {
        await api.delete(`/horarios/${id}`);
    }
};

export default horarioService;
