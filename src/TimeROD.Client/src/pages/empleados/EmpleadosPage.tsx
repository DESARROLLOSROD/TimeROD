import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Plus, Edit2, Trash2, Users, Search } from 'lucide-react';
import empleadoService from '../../services/empleadoService';
import type { EmpleadoDto } from '../../types/empleado';

export default function EmpleadosPage() {
    const [empleados, setEmpleados] = useState<EmpleadoDto[]>([]);
    const [filteredEmpleados, setFilteredEmpleados] = useState<EmpleadoDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        loadEmpleados();
    }, []);

    useEffect(() => {
        // Filter logic
        const lowerTerm = searchTerm.toLowerCase();
        const filtered = empleados.filter(e =>
            e.nombre.toLowerCase().includes(lowerTerm) ||
            e.apellidos.toLowerCase().includes(lowerTerm) ||
            e.numeroEmpleado.toLowerCase().includes(lowerTerm) ||
            (e.empresaNombre && e.empresaNombre.toLowerCase().includes(lowerTerm))
        );
        setFilteredEmpleados(filtered);
    }, [searchTerm, empleados]);

    const loadEmpleados = async () => {
        try {
            const data = await empleadoService.getAll();
            setEmpleados(data);
            setFilteredEmpleados(data);
        } catch (err) {
            setError('Error al cargar los empleados.');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id: number) => {
        if (!window.confirm('¿Estás seguro de eliminar este empleado?')) return;

        try {
            await empleadoService.delete(id);
            const newEmpleados = empleados.filter(e => e.id !== id);
            setEmpleados(newEmpleados);
            setFilteredEmpleados(newEmpleados); // Update filtered list too
        } catch (err) {
            alert('Error al eliminar el empleado. Verifique si tiene asistencias registradas.');
        }
    };

    if (loading) return <div className="text-center py-10">Cargando empleados...</div>;

    return (
        <div className="space-y-6">
            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900">Empleados</h1>
                    <p className="mt-1 text-sm text-gray-500">Gestión de personal y asignación de datos.</p>
                </div>
                <Link
                    to="/empleados/nuevo"
                    className="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                >
                    <Plus className="-ml-1 mr-2 h-5 w-5" />
                    Nuevo Empleado
                </Link>
            </div>

            {error && <div className="bg-red-50 p-4 rounded-md text-red-700">{error}</div>}

            <div className="relative rounded-md shadow-sm">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                    <Search className="h-5 w-5 text-gray-400" />
                </div>
                <input
                    type="text"
                    className="focus:ring-blue-500 focus:border-blue-500 block w-full pl-10 sm:text-sm border-gray-300 rounded-md p-2 border"
                    placeholder="Buscar por nombre, número o empresa..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                />
            </div>

            <div className="bg-white shadow overflow-hidden sm:rounded-md">
                <ul className="divide-y divide-gray-200">
                    {filteredEmpleados.map((empleado) => (
                        <li key={empleado.id}>
                            <div className="px-4 py-4 sm:px-6 flex items-center justify-between">
                                <div className="flex items-center">
                                    <div className="flex-shrink-0">
                                        <div className="h-10 w-10 rounded-full bg-green-100 flex items-center justify-center">
                                            <Users className="h-6 w-6 text-green-600" />
                                        </div>
                                    </div>
                                    <div className="ml-4">
                                        <h3 className="text-lg font-medium text-green-600 truncate">
                                            {empleado.nombre} {empleado.apellidos}
                                            <span className="ml-2 text-xs text-gray-400">({empleado.numeroEmpleado})</span>
                                        </h3>
                                        <div className="flex flex-col sm:flex-row sm:items-center text-sm text-gray-500">
                                            {empleado.empresaNombre && (
                                                <span className="mr-3"><span className="font-medium">Empresa:</span> {empleado.empresaNombre}</span>
                                            )}
                                            {empleado.areaNombre && (
                                                <span className="mr-3"><span className="font-medium">Área:</span> {empleado.areaNombre}</span>
                                            )}
                                        </div>
                                        <div className="flex items-center text-sm text-gray-500 mt-1">
                                            <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${empleado.activo ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                                                {empleado.activo ? 'Activo' : 'Inactivo'}
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div className="flex space-x-2">
                                    <Link
                                        to={`/empleados/editar/${empleado.id}`}
                                        className="p-2 text-gray-400 hover:text-blue-600"
                                        title="Editar"
                                    >
                                        <Edit2 className="h-5 w-5" />
                                    </Link>
                                    <button
                                        onClick={() => handleDelete(empleado.id)}
                                        className="p-2 text-gray-400 hover:text-red-600"
                                        title="Eliminar"
                                    >
                                        <Trash2 className="h-5 w-5" />
                                    </button>
                                </div>
                            </div>
                        </li>
                    ))}
                    {filteredEmpleados.length === 0 && (
                        <li className="px-4 py-8 text-center text-gray-500">
                            {empleados.length === 0 ? "No hay empleados registrados." : "No se encontraron empleados con ese criterio."}
                        </li>
                    )}
                </ul>
            </div>
        </div>
    );
}
