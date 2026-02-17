import api from './api';
import type { AsistenciaDto, RegistroEntradaDto, RegistroSalidaDto, UpdateAsistenciaDto } from '../types/asistencia';
import type { AsistenciaReporte } from '../types/reporte';

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
    },

    getReporte: async (fechaInicio?: string, fechaFin?: string, empresaId?: number): Promise<AsistenciaReporte> => {
        const params: any = {};
        if (fechaInicio) params.fechaInicio = fechaInicio;
        if (fechaFin) params.fechaFin = fechaFin;
        if (empresaId) params.empresaId = empresaId;

        const response = await api.get<AsistenciaReporte>('/asistencias/reporte', { params });
        return response.data;
    },

    downloadReporteExcel: async (fechaInicio?: string, fechaFin?: string, empresaId?: number): Promise<void> => {
        const params: any = {};
        if (fechaInicio) params.fechaInicio = fechaInicio;
        if (fechaFin) params.fechaFin = fechaFin;
        if (empresaId) params.empresaId = empresaId;

        const response = await api.get('/asistencias/reporte/excel', {
            params,
            responseType: 'blob'
        });

        const url = window.URL.createObjectURL(new Blob([response.data]));
        const link = document.createElement('a');
        link.href = url;
        link.setAttribute('download', `Reporte_TimeROD_${new Date().toISOString().slice(0, 10)}.xlsx`);
        document.body.appendChild(link);
        link.click();
        link.remove();
    },

    downloadReportePdf: async (fechaInicio?: string, fechaFin?: string, empresaId?: number): Promise<void> => {
        const params: any = {};
        if (fechaInicio) params.fechaInicio = fechaInicio;
        if (fechaFin) params.fechaFin = fechaFin;
        if (empresaId) params.empresaId = empresaId;

        const response = await api.get('/asistencias/reporte/pdf', {
            params,
            responseType: 'blob'
        });

        const url = window.URL.createObjectURL(new Blob([response.data]));
        const link = document.createElement('a');
        link.href = url;
        link.setAttribute('download', `Reporte_TimeROD_${new Date().toISOString().slice(0, 10)}.pdf`);
        document.body.appendChild(link);
        link.click();
        link.remove();
    }
};

export default asistenciaService;
