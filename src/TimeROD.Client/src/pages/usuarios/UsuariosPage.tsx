import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Plus, Edit2, Trash2, User } from 'lucide-react';
import usuarioService from '../../services/usuarioService';
import type { UsuarioDto } from '../../types/usuario';

export default function UsuariosPage() {
    const [usuarios, setUsuarios] = useState<UsuarioDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        loadUsuarios();
    }, []);

    const loadUsuarios = async () => {
        try {
            const data = await usuarioService.getAll();
            setUsuarios(data);
        } catch (err) {
            setError('Error al cargar los usuarios.');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id: number) => {
        if (!window.confirm('¿Estás seguro de eliminar este usuario?')) return;

        try {
            await usuarioService.delete(id);
            setUsuarios(usuarios.filter(u => u.id !== id));
        } catch (err) {
            alert('Error al eliminar el usuario.');
        }
    };

    if (loading) return <div className="text-center py-10">Cargando usuarios...</div>;

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900">Usuarios</h1>
                    <p className="mt-1 text-sm text-gray-500">Gestión de usuarios y accesos al sistema.</p>
                </div>
                <Link
                    to="/usuarios/nuevo"
                    className="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                >
                    <Plus className="-ml-1 mr-2 h-5 w-5" />
                    Nuevo Usuario
                </Link>
            </div>

            {error && <div className="bg-red-50 p-4 rounded-md text-red-700">{error}</div>}

            <div className="bg-white shadow overflow-hidden sm:rounded-md">
                <ul className="divide-y divide-gray-200">
                    {usuarios.map((usuario) => (
                        <li key={usuario.id}>
                            <div className="px-4 py-4 sm:px-6 flex items-center justify-between">
                                <div className="flex items-center">
                                    <div className="flex-shrink-0">
                                        <div className="h-10 w-10 rounded-full bg-indigo-100 flex items-center justify-center">
                                            <User className="h-6 w-6 text-indigo-600" />
                                        </div>
                                    </div>
                                    <div className="ml-4">
                                        <h3 className="text-lg font-medium text-indigo-600 truncate">{usuario.nombreCompleto}</h3>
                                        <div className="flex items-center text-sm text-gray-500">
                                            <span className="font-medium mr-2">Email:</span> {usuario.email}
                                        </div>
                                        <div className="flex items-center text-sm text-gray-500 mt-1">
                                            <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-gray-100 text-gray-800 mr-2`}>
                                                {usuario.rol}
                                            </span>
                                            <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${usuario.activo ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                                                {usuario.activo ? 'Activo' : 'Inactivo'}
                                            </span>
                                            {usuario.empresaNombre && (
                                                <span className="ml-2 text-xs text-gray-400">({usuario.empresaNombre})</span>
                                            )}
                                        </div>
                                    </div>
                                </div>
                                <div className="flex space-x-2">
                                    <Link
                                        to={`/usuarios/editar/${usuario.id}`}
                                        className="p-2 text-gray-400 hover:text-blue-600"
                                        title="Editar"
                                    >
                                        <Edit2 className="h-5 w-5" />
                                    </Link>
                                    <button
                                        onClick={() => handleDelete(usuario.id)}
                                        className="p-2 text-gray-400 hover:text-red-600"
                                        title="Eliminar"
                                    >
                                        <Trash2 className="h-5 w-5" />
                                    </button>
                                </div>
                            </div>
                        </li>
                    ))}
                    {usuarios.length === 0 && (
                        <li className="px-4 py-8 text-center text-gray-500">
                            No hay usuarios registrados.
                        </li>
                    )}
                </ul>
            </div>
        </div>
    );
}
