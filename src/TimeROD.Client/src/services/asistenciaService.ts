import api from './api';
import type { AsistenciaDto, RegistroEntradaDto, RegistroSalidaDto, UpdateAsistenciaDto } from '../types/asistencia';

const asistenciaService = {
    getAll: async (fechaInicio?: string, fechaFin?: string, empleadoId?: number): Promise<AsistenciaDto[]> => {
        const params: any = {};
        if (fechaInicio) params.fechaInicio = fechaInicio;
        if (fechaFin) params.fechaFin = fechaFin;
        if (empleadoId) params.empleadoId = empleadoId;

        const response = await api.get<AsistenciaDto[]>('/asistencias', { params });
        return response.data;
    },

    getById: async (id: number): Promise<AsistenciaDto> => {
        const response = await api.get<AsistenciaDto>(`/asistencias/${id}`);
        return response.data;
    },

    getByEmpleado: async (empleadoId: number): Promise<AsistenciaDto[]> => {
        const response = await api.get<AsistenciaDto[]>(`/asistencias/empleado/${empleadoId}`);
        return response.data;
    },

    registrarEntrada: async (dto: RegistroEntradaDto): Promise<AsistenciaDto> => {
        const response = await api.post<AsistenciaDto>('/asistencias/entrada', dto);
        return response.data;
    },

    registrarSalida: async (dto: RegistroSalidaDto): Promise<AsistenciaDto> => {
        const response = await api.post<AsistenciaDto>('/asistencias/salida', dto);
        return response.data;
    },

    update: async (id: number, dto: UpdateAsistenciaDto): Promise<void> => {
        await api.put(`/asistencias/${id}`, dto);
    },

    delete: async (id: number): Promise<void> => {
        await api.delete(`/asistencias/${id}`);
    }
};

export default asistenciaService;
