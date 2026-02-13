import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { ArrowLeft, Save } from 'lucide-react';
import empresaService from '../../services/empresaService';
import type { CreateEmpresaDto, UpdateEmpresaDto } from '../../types/empresa';

export default function EmpresaFormPage() {
    const { id } = useParams<{ id: string }>();
    const isEditing = !!id;
    const navigate = useNavigate();
    const { register, handleSubmit, formState: { errors }, setValue } = useForm<CreateEmpresaDto>();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    useEffect(() => {
        if (isEditing) {
            loadEmpresa(Number(id));
        }
    }, [id]);

    const loadEmpresa = async (empresaId: number) => {
        try {
            setLoading(true);
            const data = await empresaService.getById(empresaId);
            setValue('nombre', data.nombre);
            setValue('rfc', data.rfc);
            setValue('direccion', data.direccion);
            setValue('telefono', data.telefono);
            setValue('activo', data.activo);
        } catch (err) {
            setError('Error al cargar la empresa.');
        } finally {
            setLoading(false);
        }
    };

    const onSubmit = async (data: CreateEmpresaDto | UpdateEmpresaDto) => {
        setLoading(true);
        setError('');
        try {
            if (isEditing) {
                await empresaService.update(Number(id), data as UpdateEmpresaDto);
            } else {
                await empresaService.create(data as CreateEmpresaDto);
            }
            navigate('/empresas');
        } catch (err) {
            setError('Error al guardar la empresa. Verifique los datos.');
        } finally {
            setLoading(false);
        }
    };

    if (loading && isEditing) return <div className="text-center py-10">Cargando...</div>;

    return (
        <div className="max-w-2xl mx-auto">
            <div className="mb-6 flex items-center justify-between">
                <div className="flex items-center">
                    <Link to="/empresas" className="mr-4 text-gray-500 hover:text-gray-700">
                        <ArrowLeft className="h-6 w-6" />
                    </Link>
                    <h1 className="text-2xl font-bold text-gray-900">
                        {isEditing ? 'Editar Empresa' : 'Nueva Empresa'}
                    </h1>
                </div>
            </div>

            {error && <div className="mb-4 bg-red-50 p-4 rounded-md text-red-700">{error}</div>}

            <div className="bg-white shadow rounded-lg p-6">
                <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                    <div>
                        <label className="block text-sm font-medium text-gray-700">Nombre de la Empresa</label>
                        <input
                            type="text"
                            {...register('nombre', { required: 'El nombre es obligatorio' })}
                            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                        />
                        {errors.nombre && <p className="mt-1 text-sm text-red-600">{errors.nombre.message}</p>}
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700">RFC</label>
                        <input
                            type="text"
                            {...register('rfc', { required: 'El RFC es obligatorio' })}
                            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                        />
                        {errors.rfc && <p className="mt-1 text-sm text-red-600">{errors.rfc.message}</p>}
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700">Dirección</label>
                        <input
                            type="text"
                            {...register('direccion')}
                            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700">Teléfono</label>
                        <input
                            type="text"
                            {...register('telefono')}
                            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                        />
                    </div>

                    <div className="flex items-center">
                        <input
                            id="activo"
                            type="checkbox"
                            {...register('activo')}
                            className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                        />
                        <label htmlFor="activo" className="ml-2 block text-sm text-gray-900">
                            Activa
                        </label>
                    </div>

                    <div className="flex justify-end pt-4">
                        <Link
                            to="/empresas"
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
