import { useEffect, useState } from 'react';
import { Search, Calendar, User, Clock, TrendingUp, AlertCircle, CheckCircle, Download, Edit2 } from 'lucide-react';
import asistenciaService from '../../services/asistenciaService';
import empleadoService from '../../services/empleadoService';
import AsistenciaEditModal from '../../components/asistencias/AsistenciaEditModal';
import type { AsistenciaDto, UpdateAsistenciaDto } from '../../types/asistencia';
import type { EmpleadoDto } from '../../types/empleado';
import type { AsistenciaReporte } from '../../types/reporte';

export default function AsistenciasPage() {
    const [asistencias, setAsistencias] = useState<AsistenciaDto[]>([]);
    const [reporte, setReporte] = useState<AsistenciaReporte | null>(null);
    const [empleados, setEmpleados] = useState<EmpleadoDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    // Edit Modal State
    const [isEditModalOpen, setIsEditModalOpen] = useState(false);
    const [editingAsistencia, setEditingAsistencia] = useState<AsistenciaDto | null>(null);

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
            const now = new Date();
            const year = now.getFullYear();
            const month = String(now.getMonth() + 1).padStart(2, '0');
            const day = String(now.getDate()).padStart(2, '0');
            const today = `${year}-${month}-${day}`;
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
            const reportData = await asistenciaService.getReporte(
                start || fechaInicio,
                end || fechaFin,
                undefined // EmpresaId not used yet in this context
            );

            // Filter by employee locally if needed, or pass to service if the endpoint supported it (it does supports filters but getReporte might look different)
            // The endpoint getReporte returns all assistances in the date range.
            // If we want to filter by employee, we should do it in the service call or filter the result.
            // The service definition for getReporte we added: (fechaInicio, fechaFin, empresaId).
            // It seems the backend getReporte doesn't support employeeId filter directly in the controller?
            // Let's check Controller: GetReporteAsync(DateTime? fechaInicio, DateTime? fechaFin, int? empresaId)
            // Correct, it does NOT support employeeId. So we must filter locally or use the other endpoint for the list.

            // However, to get the metrics right for a specific employee, we should probably add employeeId to the backend functionality eventually.
            // For now, let's use the report endpoint for the metrics of ALL (or filtered by date), 
            // BUT if an employee is selected, we might want to fallback to the old method OR filter the report result.

            // Let's stick to the plan: Use Report Endpoint. 
            // If employee is selected, we filter the `asistencias` array from the report?
            // That would make the "Total metrics" incorrect for the specific employee view.

            // PROPOSAL: Use getReporte for the top cards (Global/Company view context) 
            // AND use GetAll/GetByEmpleado for the list if specific interactions are needed?
            // OR just display what the report returns.

            // Let's perform client-side filtering on the report data if an employee is selected, 
            // so the metrics update dynamically!

            let filteredAsistencias = reportData.asistencias;
            const selectedEmpId = empId ? Number(empId) : (empleadoId ? Number(empleadoId) : undefined);

            if (selectedEmpId) {
                filteredAsistencias = filteredAsistencias.filter(a => a.empleadoId === selectedEmpId);
                // Re-calculate metrics for client-side display
                const totalHoras = filteredAsistencias.reduce((sum, a) => sum + (a.horasTrabajadas || 0), 0);
                const llegadasTardias = filteredAsistencias.filter(a => a.llegadaTardia).length;

                setReporte({
                    ...reportData,
                    totalRegistros: filteredAsistencias.length,
                    totalHorasTrabajadas: totalHoras,
                    llegadasTardias: llegadasTardias,
                    asistencias: filteredAsistencias
                });
                setAsistencias(filteredAsistencias);
            } else {
                setReporte(reportData);
                setAsistencias(reportData.asistencias);
            }

        } catch (err) {
            setError('Error al cargar reporte y asistencias.');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const handleSearch = (e: React.FormEvent) => {
        e.preventDefault();
        loadAsistencias();
    };

    const handleEdit = (asistencia: AsistenciaDto) => {
        setEditingAsistencia(asistencia);
        setIsEditModalOpen(true);
    };

    const handleCloseModal = () => {
        setIsEditModalOpen(false);
        setEditingAsistencia(null);
    };

    const handleSaveEdit = async (id: number, data: UpdateAsistenciaDto) => {
        try {
            await asistenciaService.update(id, data);
            handleCloseModal();
            loadAsistencias(); // Refresh list
        } catch (err) {
            console.error('Error updating asistencia:', err);
            setError('Error al actualizar la asistencia.');
            setTimeout(() => setError(''), 3000);
        }
    };

    return (
        <div className="space-y-6">
            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900">Historial de Asistencias</h1>
                    <p className="mt-1 text-sm text-gray-500">Consulta de entradas y salidas del personal.</p>
                </div>
                <div className="flex gap-2">
                    <button
                        onClick={() => asistenciaService.downloadReporteExcel(fechaInicio, fechaFin, undefined)}
                        className="inline-flex items-center px-4 py-2 border border-blue-300 shadow-sm text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                    >
                        <Download className="-ml-1 mr-2 h-5 w-5 text-green-600" />
                        Excel
                    </button>
                    <button
                        onClick={() => asistenciaService.downloadReportePdf(fechaInicio, fechaFin, undefined)}
                        className="inline-flex items-center px-4 py-2 border border-blue-300 shadow-sm text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                    >
                        <Download className="-ml-1 mr-2 h-5 w-5 text-red-600" />
                        PDF
                    </button>
                </div>
            </div>

            <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
                <div className="bg-white overflow-hidden shadow rounded-lg">
                    <div className="p-5">
                        <div className="flex items-center">
                            <div className="flex-shrink-0">
                                <Clock className="h-6 w-6 text-gray-400" aria-hidden="true" />
                            </div>
                            <div className="ml-5 w-0 flex-1">
                                <dl>
                                    <dt className="text-sm font-medium text-gray-500 truncate">Total Horas</dt>
                                    <dd>
                                        <div className="text-lg font-medium text-gray-900">
                                            {reporte?.totalHorasTrabajadas.toFixed(1) || '0.0'}
                                        </div>
                                    </dd>
                                </dl>
                            </div>
                        </div>
                    </div>
                </div>

                <div className="bg-white overflow-hidden shadow rounded-lg">
                    <div className="p-5">
                        <div className="flex items-center">
                            <div className="flex-shrink-0">
                                <AlertCircle className="h-6 w-6 text-red-400" aria-hidden="true" />
                            </div>
                            <div className="ml-5 w-0 flex-1">
                                <dl>
                                    <dt className="text-sm font-medium text-gray-500 truncate">Llegadas Tardías</dt>
                                    <dd>
                                        <div className="text-lg font-medium text-gray-900">
                                            {reporte?.llegadasTardias || 0}
                                        </div>
                                    </dd>
                                </dl>
                            </div>
                        </div>
                    </div>
                </div>

                <div className="bg-white overflow-hidden shadow rounded-lg">
                    <div className="p-5">
                        <div className="flex items-center">
                            <div className="flex-shrink-0">
                                <CheckCircle className="h-6 w-6 text-green-400" aria-hidden="true" />
                            </div>
                            <div className="ml-5 w-0 flex-1">
                                <dl>
                                    <dt className="text-sm font-medium text-gray-500 truncate">Asistencias</dt>
                                    <dd>
                                        <div className="text-lg font-medium text-gray-900">
                                            {reporte?.totalRegistros || 0}
                                        </div>
                                    </dd>
                                </dl>
                            </div>
                        </div>
                    </div>
                </div>

                <div className="bg-white overflow-hidden shadow rounded-lg">
                    <div className="p-5">
                        <div className="flex items-center">
                            <div className="flex-shrink-0">
                                <TrendingUp className="h-6 w-6 text-blue-400" aria-hidden="true" />
                            </div>
                            <div className="ml-5 w-0 flex-1">
                                <dl>
                                    <dt className="text-sm font-medium text-gray-500 truncate">Promedio Horas/Día</dt>
                                    <dd>
                                        <div className="text-lg font-medium text-gray-900">
                                            {reporte?.promedioHorasPorDia?.toFixed(1) || '0.0'}
                                        </div>
                                    </dd>
                                </dl>
                            </div>
                        </div>
                    </div>
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
                                                <span className="mr-4">{new Date(asistencia.fecha + 'T00:00:00').toLocaleDateString()}</span>

                                                <Clock className="flex-shrink-0 mr-1.5 h-4 w-4 text-gray-400" />
                                                <span className="mr-2 text-green-600 font-medium">Entrada: {asistencia.horaEntrada}</span>
                                                {asistencia.horaSalida && (
                                                    <span className="text-red-600 font-medium">Salida: {asistencia.horaSalida}</span>
                                                )}
                                            </div>
                                        </div>
                                    </div>
                                    <div className="flex flex-col items-end">
                                        {asistencia.llegadaTardia && (
                                            <span className="mb-1 px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-red-100 text-red-800">
                                                Tarde ({asistencia.minutosRetraso} min)
                                            </span>
                                        )}
                                        {asistencia.salidaAnticipada && (
                                            <span className="mb-1 px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-orange-100 text-orange-800">
                                                Salida Anticipada ({asistencia.minutosAnticipados} min)
                                            </span>
                                        )}
                                        <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${asistencia.tipo === 'Normal' ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'
                                            }`}>
                                            {asistencia.tipo}
                                        </span>
                                        {asistencia.observaciones && (
                                            <span className="text-xs text-gray-400 mt-1 max-w-xs truncate" title={asistencia.observaciones}>
                                                {asistencia.observaciones}
                                            </span>
                                        )}
                                        <button
                                            onClick={() => handleEdit(asistencia)}
                                            className="mt-2 inline-flex items-center p-1 border border-transparent rounded-full shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                                            title="Editar Asistencia"
                                        >
                                            <Edit2 className="h-4 w-4" />
                                        </button>
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

            {
                editingAsistencia && (
                    <AsistenciaEditModal
                        asistencia={editingAsistencia}
                        isOpen={isEditModalOpen}
                        onClose={handleCloseModal}
                        onSave={handleSaveEdit}
                    />
                )
            }
        </div >
    );
}
