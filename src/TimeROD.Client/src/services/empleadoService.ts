import api from './api';
import type { EmpleadoDto, CreateEmpleadoDto, UpdateEmpleadoDto } from '../types/empleado';

const empleadoService = {
    getAll: async (): Promise<EmpleadoDto[]> => {
        const response = await api.get<EmpleadoDto[]>('/empleados');
        return response.data;
    },

    getAllByEmpresa: async (empresaId: number): Promise<EmpleadoDto[]> => {
        const response = await api.get<EmpleadoDto[]>(`/empleados/empresa/${empresaId}`);
        return response.data;
    },

    getAllByArea: async (areaId: number): Promise<EmpleadoDto[]> => {
        const response = await api.get<EmpleadoDto[]>(`/empleados/area/${areaId}`);
        return response.data;
    },

    getById: async (id: number): Promise<EmpleadoDto> => {
        const response = await api.get<EmpleadoDto>(`/empleados/${id}`);
        return response.data;
    },

    create: async (empleado: CreateEmpleadoDto): Promise<EmpleadoDto> => {
        const response = await api.post<EmpleadoDto>('/empleados', empleado);
        return response.data;
    },

    update: async (id: number, empleado: UpdateEmpleadoDto): Promise<void> => {
        await api.put(`/empleados/${id}`, empleado);
    },

    delete: async (id: number): Promise<void> => {
        await api.delete(`/empleados/${id}`);
    }
};

export default empleadoService;
