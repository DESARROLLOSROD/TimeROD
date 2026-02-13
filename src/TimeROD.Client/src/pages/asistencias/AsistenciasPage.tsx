import { useEffect, useState } from 'react';
import { Search, Calendar, User, Clock } from 'lucide-react';
import asistenciaService from '../../services/asistenciaService';
import empleadoService from '../../services/empleadoService';
import type { AsistenciaDto } from '../../types/asistencia';
import type { EmpleadoDto } from '../../types/empleado';

export default function AsistenciasPage() {
    const [asistencias, setAsistencias] = useState<AsistenciaDto[]>([]);
    const [empleados, setEmpleados] = useState<EmpleadoDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    // Filters
    const [fechaInicio, setFechaInicio] = useState('');
    const [fechaFin, setFechaFin] = useState('');
    const [empleadoId, setEmpleadoId] = useState('');

    useEffect(() => {
        loadInitialData();
    }, []);

    const loadInitialData = async () => {
        try {
            const [empleadosData] = await Promise.all([
                empleadoService.getAll()
            ]);
            setEmpleados(empleadosData);
            // Load today's attendances by default? Or just all? Let's load recent.
            // For now, load all without filters or maybe last 7 days.
            // existing implementation of getAll allows optional params.
            const today = new Date().toISOString().split('T')[0];
            setFechaInicio(today);
            setFechaFin(today);
            await loadAsistencias(today, today);
        } catch (err) {
            console.error(err);
            setError('Error al cargar datos iniciales.');
        } finally {
            setLoading(false);
        }
    };

    const loadAsistencias = async (start?: string, end?: string, empId?: string) => {
        setLoading(true);
        try {
            const data = await asistenciaService.getAll(
                start || fechaInicio,
                end || fechaFin,
                empId ? Number(empId) : (empleadoId ? Number(empleadoId) : undefined)
            );
            setAsistencias(data);
        } catch (err) {
            setError('Error al cargar historial de asistencias.');
        } finally {
            setLoading(false);
        }
    };

    const handleSearch = (e: React.FormEvent) => {
        e.preventDefault();
        loadAsistencias();
    };

    return (
        <div className="space-y-6">
            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900">Historial de Asistencias</h1>
                    <p className="mt-1 text-sm text-gray-500">Consulta de entradas y salidas del personal.</p>
                </div>
            </div>

            <div className="bg-white shadow rounded-lg p-4">
                <form onSubmit={handleSearch} className="grid grid-cols-1 md:grid-cols-4 gap-4 items-end">
                    <div>
                        <label className="block text-sm font-medium text-gray-700">Fecha Inicio</label>
                        <input
                            type="date"
                            value={fechaInicio}
                            onChange={(e) => setFechaInicio(e.target.value)}
                            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700">Fecha Fin</label>
                        <input
                            type="date"
                            value={fechaFin}
                            onChange={(e) => setFechaFin(e.target.value)}
                            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700">Empleado</label>
                        <select
                            value={empleadoId}
                            onChange={(e) => setEmpleadoId(e.target.value)}
                            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                        >
                            <option value="">Todos</option>
                            {empleados.map(e => (
                                <option key={e.id} value={e.id}>{e.nombre} {e.apellidos}</option>
                            ))}
                        </select>
                    </div>
                    <div>
                        <button
                            type="submit"
                            className="w-full inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                        >
                            <Search className="h-5 w-5 mr-2" />
                            Buscar
                        </button>
                    </div>
                </form>
            </div>

            {error && <div className="bg-red-50 p-4 rounded-md text-red-700">{error}</div>}

            <div className="bg-white shadow overflow-hidden sm:rounded-md">
                {loading ? (
                    <div className="text-center py-10">Cargando...</div>
                ) : (
                    <ul className="divide-y divide-gray-200">
                        {asistencias.map((asistencia) => (
                            <li key={asistencia.id} className="px-4 py-4 sm:px-6">
                                <div className="flex items-center justify-between">
                                    <div className="flex items-center">
                                        <div className="flex-shrink-0">
                                            <div className="h-10 w-10 rounded-full bg-blue-100 flex items-center justify-center">
                                                <User className="h-6 w-6 text-blue-600" />
                                            </div>
                                        </div>
                                        <div className="ml-4">
                                            <h3 className="text-lg font-medium text-gray-900">
                                                {asistencia.empleadoNombreCompleto || `Empleado #${asistencia.empleadoId}`}
                                                <span className="ml-2 text-xs text-gray-500">
                                                    ({asistencia.empresaNombre} - {asistencia.areaNombre})
                                                </span>
                                            </h3>
                                            <div className="flex items-center text-sm text-gray-500 mt-1">
                                                <Calendar className="flex-shrink-0 mr-1.5 h-4 w-4 text-gray-400" />
                                                <span className="mr-4">{new Date(asistencia.fecha).toLocaleDateString()}</span>

                                                <Clock className="flex-shrink-0 mr-1.5 h-4 w-4 text-gray-400" />
                                                <span className="mr-2 text-green-600 font-medium">Entrada: {asistencia.horaEntrada}</span>
                                                {asistencia.horaSalida && (
                                                    <span className="text-red-600 font-medium">Salida: {asistencia.horaSalida}</span>
                                                )}
                                            </div>
                                        </div>
                                    </div>
                                    <div className="flex flex-col items-end">
                                        <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${asistencia.tipo === 'Normal' ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'
                                            }`}>
                                            {asistencia.tipo}
                                        </span>
                                        {asistencia.observaciones && (
                                            <span className="text-xs text-gray-400 mt-1 max-w-xs truncate" title={asistencia.observaciones}>
                                                {asistencia.observaciones}
                                            </span>
                                        )}
                                    </div>
                                </div>
                            </li>
                        ))}
                        {asistencias.length === 0 && (
                            <li className="px-4 py-8 text-center text-gray-500">
                                No se encontraron registros para los filtros seleccionados.
                            </li>
                        )}
                    </ul>
                )}
            </div>
        </div>
    );
}
