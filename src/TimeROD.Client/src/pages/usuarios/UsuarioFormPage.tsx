import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { ArrowLeft, Save } from 'lucide-react';
import usuarioService from '../../services/usuarioService';
import empresaService from '../../services/empresaService';
import type { CreateUsuarioDto, UpdateUsuarioDto } from '../../types/usuario';
import type { EmpresaDto } from '../../types/empresa';

export default function UsuarioFormPage() {
    const { id } = useParams<{ id: string }>();
    const isEditing = !!id;
    const navigate = useNavigate();
    const { register, handleSubmit, formState: { errors }, setValue } = useForm<CreateUsuarioDto & UpdateUsuarioDto>();
    const [loading, setLoading] = useState(false);
    const [empresas, setEmpresas] = useState<EmpresaDto[]>([]);
    const [error, setError] = useState('');

    useEffect(() => {
        loadDependencies();
        if (isEditing) {
            loadUsuario(Number(id));
        }
    }, [id]);

    const loadDependencies = async () => {
        try {
            const empresasData = await empresaService.getAll();
            setEmpresas(empresasData);
        } catch (err) {
            console.error('Error loading dependencies', err);
        }
    };

    const loadUsuario = async (userId: number) => {
        try {
            setLoading(true);
            const data = await usuarioService.getById(userId);
            setValue('nombreCompleto', data.nombreCompleto);
            setValue('email', data.email);
            setValue('rol', data.rol);
            setValue('empresaId', data.empresaId);
            setValue('activo', data.activo);
        } catch (err) {
            setError('Error al cargar el usuario.');
        } finally {
            setLoading(false);
        }
    };

    const onSubmit = async (data: any) => {
        setLoading(true);
        setError('');
        try {
            // Convert empresaId to number
            data.empresaId = Number(data.empresaId);

            if (isEditing) {
                // If password is empty, don't send it for update
                if (!data.password) delete data.password;
                await usuarioService.update(Number(id), data as UpdateUsuarioDto);
            } else {
                await usuarioService.create(data as CreateUsuarioDto);
            }
            navigate('/usuarios');
        } catch (err) {
            setError('Error al guardar el usuario. Verifique los datos.');
        } finally {
            setLoading(false);
        }
    };

    if (loading && isEditing) return <div className="text-center py-10">Cargando...</div>;

    return (
        <div className="max-w-2xl mx-auto">
            <div className="mb-6 flex items-center justify-between">
                <div className="flex items-center">
                    <Link to="/usuarios" className="mr-4 text-gray-500 hover:text-gray-700">
                        <ArrowLeft className="h-6 w-6" />
                    </Link>
                    <h1 className="text-2xl font-bold text-gray-900">
                        {isEditing ? 'Editar Usuario' : 'Nuevo Usuario'}
                    </h1>
                </div>
            </div>

            {error && <div className="mb-4 bg-red-50 p-4 rounded-md text-red-700">{error}</div>}

            <div className="bg-white shadow rounded-lg p-6">
                <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                    <div>
                        <label className="block text-sm font-medium text-gray-700">Nombre Completo</label>
                        <input
                            type="text"
                            {...register('nombreCompleto', { required: 'El nombre es obligatorio' })}
                            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                        />
                        {errors.nombreCompleto && <p className="mt-1 text-sm text-red-600">{errors.nombreCompleto.message}</p>}
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700">Email</label>
                        <input
                            type="email"
                            {...register('email', { required: 'El email es obligatorio' })}
                            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                        />
                        {errors.email && <p className="mt-1 text-sm text-red-600">{errors.email.message}</p>}
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700">Contraseña {isEditing && '(Dejar en blanco para mantener actual)'}</label>
                        <input
                            type="password"
                            {...register('password', { required: !isEditing ? 'La contraseña es obligatoria' : false })}
                            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                        />
                        {errors.password && <p className="mt-1 text-sm text-red-600">{errors.password.message}</p>}
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700">Rol</label>
                        <select
                            {...register('rol', { required: 'El rol es obligatorio' })}
                            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                        >
                            <option value="">Seleccione un rol</option>
                            <option value="Admin">Administrador</option>
                            <option value="Supervisor">Supervisor</option>
                            <option value="Empleado">Empleado</option>
                            <option value="RH">Recursos Humanos</option>
                        </select>
                        {errors.rol && <p className="mt-1 text-sm text-red-600">{errors.rol.message}</p>}
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700">Empresa</label>
                        <select
                            {...register('empresaId', { required: 'La empresa es obligatoria' })}
                            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                        >
                            <option value="">Seleccione una empresa</option>
                            {empresas.map((empresa) => (
                                <option key={empresa.id} value={empresa.id}>{empresa.nombre}</option>
                            ))}
                        </select>
                        {errors.empresaId && <p className="mt-1 text-sm text-red-600">{errors.empresaId.message}</p>}
                    </div>

                    <div className="flex items-center">
                        <input
                            id="activo"
                            type="checkbox"
                            {...register('activo')}
                            className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                        />
                        <label htmlFor="activo" className="ml-2 block text-sm text-gray-900">
                            Activo
                        </label>
                    </div>

                    <div className="flex justify-end pt-4">
                        <Link
                            to="/usuarios"
                            className="bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 mr-3"
                        >
                            Cancelar
                        </Link>
                        <button
                            type="submit"
                            disabled={loading}
                            className="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50"
                        >
                            <Save className="h-5 w-5 mr-2" />
                            {loading ? 'Guardando...' : 'Guardar'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
