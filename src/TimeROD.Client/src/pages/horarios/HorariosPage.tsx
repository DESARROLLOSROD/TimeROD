import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Plus, Edit2, Trash2, Clock, Calendar } from 'lucide-react';
import horarioService from '../../services/horarioService';
import type { HorarioDto } from '../../types/horario';

export default function HorariosPage() {
    const [horarios, setHorarios] = useState<HorarioDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        loadHorarios();
    }, []);

    const loadHorarios = async () => {
        try {
            const data = await horarioService.getAll();
            setHorarios(data);
        } catch (err) {
            setError('Error al cargar los horarios.');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id: number) => {
        if (!window.confirm('¿Estás seguro de eliminar este horario?')) return;

        try {
            await horarioService.delete(id);
            setHorarios(horarios.filter(h => h.id !== id));
        } catch (err) {
            alert('Error al eliminar el horario. Es posible que esté asignado a áreas o empleados.');
        }
    };

    if (loading) return <div className="text-center py-10">Cargando horarios...</div>;

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900">Horarios</h1>
                    <p className="mt-1 text-sm text-gray-500">Gestión de turnos y horarios laborales.</p>
                </div>
                <Link
                    to="/horarios/nuevo"
                    className="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                >
                    <Plus className="-ml-1 mr-2 h-5 w-5" />
                    Nuevo Horario
                </Link>
            </div>

            {error && <div className="bg-red-50 p-4 rounded-md text-red-700">{error}</div>}

            <div className="bg-white shadow overflow-hidden sm:rounded-md">
                <ul className="divide-y divide-gray-200">
                    {horarios.map((horario) => (
                        <li key={horario.id}>
                            <div className="px-4 py-4 sm:px-6 flex items-center justify-between">
                                <div className="flex items-center">
                                    <div className="flex-shrink-0">
                                        <div className="h-10 w-10 rounded-full bg-orange-100 flex items-center justify-center">
                                            <Clock className="h-6 w-6 text-orange-600" />
                                        </div>
                                    </div>
                                    <div className="ml-4">
                                        <h3 className="text-lg font-medium text-orange-600 truncate">{horario.nombre}</h3>
                                        <div className="flex items-center text-sm text-gray-500 mt-1">
                                            <Calendar className="flex-shrink-0 mr-1.5 h-4 w-4 text-gray-400" />
                                            <span className="font-medium mr-4">
                                                {horario.horaEntrada.substring(0, 5)} - {horario.horaSalida.substring(0, 5)}
                                            </span>
                                            <span className="text-gray-400">
                                                Tolerancia: {horario.toleranciaMinutos} min
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div className="flex space-x-2">
                                    <Link
                                        to={`/horarios/editar/${horario.id}`}
                                        className="p-2 text-gray-400 hover:text-blue-600"
                                        title="Editar"
                                    >
                                        <Edit2 className="h-5 w-5" />
                                    </Link>
                                    <button
                                        onClick={() => handleDelete(horario.id)}
                                        className="p-2 text-gray-400 hover:text-red-600"
                                        title="Eliminar"
                                    >
                                        <Trash2 className="h-5 w-5" />
                                    </button>
                                </div>
                            </div>
                        </li>
                    ))}
                    {horarios.length === 0 && (
                        <li className="px-4 py-8 text-center text-gray-500">
                            No hay horarios registrados.
                        </li>
                    )}
                </ul>
            </div>
        </div>
    );
}
