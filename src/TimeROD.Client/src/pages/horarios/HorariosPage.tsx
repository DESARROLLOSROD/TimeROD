import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Plus, Edit, Trash2 } from 'lucide-react';
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
            setLoading(true);
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
        if (!window.confirm('¿Está seguro de eliminar este horario?')) return;

        try {
            await horarioService.delete(id);
            setHorarios(horarios.filter(h => h.id !== id));
        } catch (err) {
            alert('Error al eliminar el horario. Verifique que no esté asignado a áreas o empleados.');
            console.error(err);
        }
    };

    if (loading) return <div className="text-center p-10">Cargando horarios...</div>;
    if (error) return <div className="text-center p-10 text-red-500">{error}</div>;

    return (
        <div className="container mx-auto px-4 py-8">
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-3xl font-bold text-gray-800">Gestión de Horarios</h1>
                <Link
                    to="/horarios/nuevo"
                    className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded flex items-center"
                >
                    <Plus className="mr-2" />
                    Nuevo Horario
                </Link>
            </div>

            <div className="bg-white shadow-md rounded-lg overflow-hidden">
                <table className="min-w-full divide-y divide-gray-200">
                    <thead className="bg-gray-50">
                        <tr>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Nombre</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Entrada</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Salida</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Tolerancia</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Acciones</th>
                        </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                        {horarios.map((horario) => (
                            <tr key={horario.id} className="hover:bg-gray-50">
                                <td className="px-6 py-4 whitespace-nowrap">
                                    <div className="text-sm font-medium text-gray-900">{horario.nombre}</div>
                                </td>
                                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                    {horario.horaEntrada}
                                </td>
                                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                    {horario.horaSalida}
                                </td>
                                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                    {horario.toleranciaMinutos} min
                                </td>
                                <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                                    <div className="flex space-x-3">
                                        <Link
                                            to={`/horarios/editar/${horario.id}`}
                                            className="text-indigo-600 hover:text-indigo-900"
                                            title="Editar"
                                        >
                                            <Edit className="w-5 h-5" />
                                        </Link>
                                        <button
                                            onClick={() => handleDelete(horario.id)}
                                            className="text-red-600 hover:text-red-900"
                                            title="Eliminar"
                                        >
                                            <Trash2 className="w-5 h-5" />
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
                {horarios.length === 0 && (
                    <div className="text-center py-10 text-gray-500">
                        No hay horarios registrados.
                    </div>
                )}
            </div>
        </div>
    );
}
