import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Plus, Edit2, Trash2, MapPin } from 'lucide-react';
import areaService from '../../services/areaService';
import type { AreaDto } from '../../types/area';

export default function AreasPage() {
    const [areas, setAreas] = useState<AreaDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        loadAreas();
    }, []);

    const loadAreas = async () => {
        try {
            const data = await areaService.getAll();
            setAreas(data);
        } catch (err) {
            setError('Error al cargar las áreas.');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id: number) => {
        if (!window.confirm('¿Estás seguro de eliminar esta área?')) return;

        try {
            await areaService.delete(id);
            setAreas(areas.filter(a => a.id !== id));
        } catch (err) {
            alert('Error al eliminar el área. Es posible que tenga empleados asociados.');
        }
    };

    if (loading) return <div className="text-center py-10">Cargando áreas...</div>;

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900">Áreas</h1>
                    <p className="mt-1 text-sm text-gray-500">Gestión de áreas y departamentos por empresa.</p>
                </div>
                <Link
                    to="/areas/nueva"
                    className="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                >
                    <Plus className="-ml-1 mr-2 h-5 w-5" />
                    Nueva Área
                </Link>
            </div>

            {error && <div className="bg-red-50 p-4 rounded-md text-red-700">{error}</div>}

            <div className="bg-white shadow overflow-hidden sm:rounded-md">
                <ul className="divide-y divide-gray-200">
                    {areas.map((area) => (
                        <li key={area.id}>
                            <div className="px-4 py-4 sm:px-6 flex items-center justify-between">
                                <div className="flex items-center">
                                    <div className="flex-shrink-0">
                                        <div className="h-10 w-10 rounded-full bg-purple-100 flex items-center justify-center">
                                            <MapPin className="h-6 w-6 text-purple-600" />
                                        </div>
                                    </div>
                                    <div className="ml-4">
                                        <h3 className="text-lg font-medium text-purple-600 truncate">{area.nombre}</h3>
                                        <div className="flex items-center text-sm text-gray-500">
                                            <span className="font-medium mr-2">Empresa:</span> {area.empresaNombre || 'N/A'}
                                        </div>
                                        {area.descripcion && (
                                            <div className="text-sm text-gray-400 mt-1">{area.descripcion}</div>
                                        )}
                                        <div className="flex items-center text-sm text-gray-500 mt-1">
                                            <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${area.activo ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                                                {area.activo ? 'Activa' : 'Inactiva'}
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div className="flex space-x-2">
                                    <Link
                                        to={`/areas/editar/${area.id}`}
                                        className="p-2 text-gray-400 hover:text-blue-600"
                                        title="Editar"
                                    >
                                        <Edit2 className="h-5 w-5" />
                                    </Link>
                                    <button
                                        onClick={() => handleDelete(area.id)}
                                        className="p-2 text-gray-400 hover:text-red-600"
                                        title="Eliminar"
                                    >
                                        <Trash2 className="h-5 w-5" />
                                    </button>
                                </div>
                            </div>
                        </li>
                    ))}
                    {areas.length === 0 && (
                        <li className="px-4 py-8 text-center text-gray-500">
                            No hay áreas registradas.
                        </li>
                    )}
                </ul>
            </div>
        </div>
    );
}
