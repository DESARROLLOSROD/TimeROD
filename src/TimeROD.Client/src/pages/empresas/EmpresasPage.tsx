import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Plus, Edit2, Trash2, Building2 } from 'lucide-react';
import empresaService from '../../services/empresaService';
import type { EmpresaDto } from '../../types/empresa';

export default function EmpresasPage() {
    const [empresas, setEmpresas] = useState<EmpresaDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        loadEmpresas();
    }, []);

    const loadEmpresas = async () => {
        try {
            const data = await empresaService.getAll();
            setEmpresas(data);
        } catch (err) {
            setError('Error al cargar las empresas.');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id: number) => {
        if (!window.confirm('¿Estás seguro de eliminar esta empresa?')) return;

        try {
            await empresaService.delete(id);
            setEmpresas(empresas.filter(e => e.id !== id));
        } catch (err) {
            alert('Error al eliminar la empresa. Es posible que tenga registros asociados.');
        }
    };

    if (loading) return <div className="text-center py-10">Cargando empresas...</div>;

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900">Empresas</h1>
                    <p className="mt-1 text-sm text-gray-500">Gestión de empresas registradas en el sistema.</p>
                </div>
                <Link
                    to="/empresas/nueva"
                    className="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                >
                    <Plus className="-ml-1 mr-2 h-5 w-5" />
                    Nueva Empresa
                </Link>
            </div>

            {error && <div className="bg-red-50 p-4 rounded-md text-red-700">{error}</div>}

            <div className="bg-white shadow overflow-hidden sm:rounded-md">
                <ul className="divide-y divide-gray-200">
                    {empresas.map((empresa) => (
                        <li key={empresa.id}>
                            <div className="px-4 py-4 sm:px-6 flex items-center justify-between">
                                <div className="flex items-center">
                                    <div className="flex-shrink-0">
                                        <div className="h-10 w-10 rounded-full bg-blue-100 flex items-center justify-center">
                                            <Building2 className="h-6 w-6 text-blue-600" />
                                        </div>
                                    </div>
                                    <div className="ml-4">
                                        <h3 className="text-lg font-medium text-blue-600 truncate">{empresa.nombre}</h3>
                                        <div className="flex items-center text-sm text-gray-500">
                                            <span className="font-medium mr-2">RFC:</span> {empresa.rfc}
                                        </div>
                                        <div className="flex items-center text-sm text-gray-500 mt-1">
                                            <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${empresa.activo ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                                                {empresa.activo ? 'Activa' : 'Inactiva'}
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div className="flex space-x-2">
                                    <Link
                                        to={`/empresas/editar/${empresa.id}`}
                                        className="p-2 text-gray-400 hover:text-blue-600"
                                        title="Editar"
                                    >
                                        <Edit2 className="h-5 w-5" />
                                    </Link>
                                    <button
                                        onClick={() => handleDelete(empresa.id)}
                                        className="p-2 text-gray-400 hover:text-red-600"
                                        title="Eliminar"
                                    >
                                        <Trash2 className="h-5 w-5" />
                                    </button>
                                </div>
                            </div>
                        </li>
                    ))}
                    {empresas.length === 0 && (
                        <li className="px-4 py-8 text-center text-gray-500">
                            No hay empresas registradas.
                        </li>
                    )}
                </ul>
            </div>
        </div>
    );
}
