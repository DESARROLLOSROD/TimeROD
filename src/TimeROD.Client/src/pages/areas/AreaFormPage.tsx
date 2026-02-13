import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { ArrowLeft, Save } from 'lucide-react';
import areaService from '../../services/areaService';
import empresaService from '../../services/empresaService';
import horarioService from '../../services/horarioService';
import type { CreateAreaDto, UpdateAreaDto } from '../../types/area';
import type { EmpresaDto } from '../../types/empresa';
import type { HorarioDto } from '../../types/horario';

export default function AreaFormPage() {
    const { id } = useParams<{ id: string }>();
    const isEditing = !!id;
    const navigate = useNavigate();
    const { register, handleSubmit, formState: { errors }, setValue } = useForm<CreateAreaDto & UpdateAreaDto>();
    const [loading, setLoading] = useState(false);
    const [empresas, setEmpresas] = useState<EmpresaDto[]>([]);
    const [horarios, setHorarios] = useState<HorarioDto[]>([]);
    const [error, setError] = useState('');

    useEffect(() => {
        loadDependencies();
        if (isEditing) {
            loadArea(Number(id));
        }
    }, [id]);

    const loadDependencies = async () => {
        try {
            const [empresasData, horariosData] = await Promise.all([
                empresaService.getAll(),
                horarioService.getAll()
            ]);
            setEmpresas(empresasData);
            setHorarios(horariosData);
        } catch (err) {
            console.error('Error loading dependencies', err);
        }
    };

    const loadArea = async (areaId: number) => {
        try {
            setLoading(true);
            const data = await areaService.getById(areaId);
            setValue('nombre', data.nombre);
            setValue('descripcion', data.descripcion);
            setValue('empresaId', data.empresaId);
            setValue('horarioId', data.horarioId);
            setValue('activo', data.activo);
        } catch (err) {
            setError('Error al cargar el área.');
        } finally {
            setLoading(false);
        }
    };

    const onSubmit = async (data: any) => {
        setLoading(true);
        setError('');
        try {
            // Convert types
            data.empresaId = Number(data.empresaId);
            if (data.horarioId) data.horarioId = Number(data.horarioId);
            else data.horarioId = null;

            if (isEditing) {
                await areaService.update(Number(id), data as UpdateAreaDto);
            } else {
                await areaService.create(data as CreateAreaDto);
            }
            navigate('/areas');
        } catch (err) {
            setError('Error al guardar el área. Verifique los datos.');
        } finally {
            setLoading(false);
        }
    };

    if (loading && isEditing) return <div className="text-center py-10">Cargando...</div>;

    return (
        <div className="max-w-2xl mx-auto">
            <div className="mb-6 flex items-center justify-between">
                <div className="flex items-center">
                    <Link to="/areas" className="mr-4 text-gray-500 hover:text-gray-700">
                        <ArrowLeft className="h-6 w-6" />
                    </Link>
                    <h1 className="text-2xl font-bold text-gray-900">
                        {isEditing ? 'Editar Área' : 'Nueva Área'}
                    </h1>
                </div>
            </div>

            {error && <div className="mb-4 bg-red-50 p-4 rounded-md text-red-700">{error}</div>}

            <div className="bg-white shadow rounded-lg p-6">
                <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                    <div>
                        <label className="block text-sm font-medium text-gray-700">Nombre del Área</label>
                        <input
                            type="text"
                            {...register('nombre', { required: 'El nombre es obligatorio' })}
                            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                        />
                        {errors.nombre && <p className="mt-1 text-sm text-red-600">{errors.nombre.message}</p>}
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700">Descripción</label>
                        <textarea
                            {...register('descripcion')}
                            rows={3}
                            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                        />
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

                    <div>
                        <label className="block text-sm font-medium text-gray-700">Horario Asignado (Opcional)</label>
                        <select
                            {...register('horarioId')}
                            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                        >
                            <option value="">-- Sin Horario Asignado --</option>
                            {horarios.map((horario) => (
                                <option key={horario.id} value={horario.id}>
                                    {horario.nombre} ({horario.horaEntrada} - {horario.horaSalida})
                                </option>
                            ))}
                        </select>
                        <p className="mt-1 text-xs text-gray-500">
                            Si se selecciona, este horario aplicará por defecto a todos los empleados de esta área.
                        </p>
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
                            to="/areas"
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
