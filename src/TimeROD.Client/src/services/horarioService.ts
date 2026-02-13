import axios from 'axios';
import type { HorarioDto, CreateHorarioDto, UpdateHorarioDto } from '../types/horario';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5169/api';

const horarioService = {
    getAll: async (): Promise<HorarioDto[]> => {
        const token = localStorage.getItem('token');
        const response = await axios.get<HorarioDto[]>(`${API_URL}/horarios`, {
            headers: { Authorization: `Bearer ${token}` }
        });
        return response.data;
    },

    getById: async (id: number): Promise<HorarioDto> => {
        const token = localStorage.getItem('token');
        const response = await axios.get<HorarioDto>(`${API_URL}/horarios/${id}`, {
            headers: { Authorization: `Bearer ${token}` }
        });
        return response.data;
    },

    create: async (horario: CreateHorarioDto): Promise<HorarioDto> => {
        const token = localStorage.getItem('token');
        const response = await axios.post<HorarioDto>(`${API_URL}/horarios`, horario, {
            headers: { Authorization: `Bearer ${token}` }
        });
        return response.data;
    },

    update: async (id: number, horario: UpdateHorarioDto): Promise<void> => {
        const token = localStorage.getItem('token');
        await axios.put(`${API_URL}/horarios/${id}`, horario, {
            headers: { Authorization: `Bearer ${token}` }
        });
    },

    delete: async (id: number): Promise<void> => {
        const token = localStorage.getItem('token');
        await axios.delete(`${API_URL}/horarios/${id}`, {
            headers: { Authorization: `Bearer ${token}` }
        });
    }
};

export default horarioService;
