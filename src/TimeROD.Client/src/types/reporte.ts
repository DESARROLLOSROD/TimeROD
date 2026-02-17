import type { AsistenciaDto } from './asistencia';

export interface AsistenciaReporte {
    fechaInicio: string;
    fechaFin: string;
    totalRegistros: number;
    totalHorasTrabajadas: number; // nullable in C# but sum returns number, let's assume number
    promedioHorasPorDia: number;
    llegadasTardias: number;
    asistencias: AsistenciaDto[];
}
