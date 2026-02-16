import { useEffect, useState } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { Save, ArrowLeft } from 'lucide-react';
import horarioService from '../../services/horarioService';
import type { CreateHorarioDto, UpdateHorarioDto } from '../../types/horario';

export default function HorarioFormPage() {
    const { id } = useParams();
    const navigate = useNavigate();
    const isEditing = !!id;
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const { register, handleSubmit, setValue, formState: { errors } } = useForm<CreateHorarioDto & { active: boolean }>();

    useEffect(() => {
        if (isEditing) {
            loadHorario();
        }
    }, [id]);

    const loadHorario = async () => {
        try {
            setLoading(true);
            const data = await horarioService.getById(Number(id));
            setValue('nombre', data.nombre);
            setValue('horaEntrada', data.horaEntrada);
            setValue('horaSalida', data.horaSalida);
            setValue('toleranciaMinutos', data.toleranciaMinutos);
            setValue('active', data.activo);
        } catch (err) {
            setError('Error al cargar el horario.');
        } finally {
            setLoading(false);
        }
    };

    const onSubmit = async (data: any) => {
        setLoading(true);
        setError('');
        try {
            // Ensure seconds are included if input type="time" omits them (browsers usually do HH:mm)
            // Backend expects TimeSpan, which can parse HH:mm or HH:mm:ss
            // We'll append :00 if missing for consisteny but it might not be strictly needed depending on .NET parsing
            const formattedData = {
                ...data,
                horaEntrada: data.horaEntrada.length === 5 ? `${data.horaEntrada}:00` : data.horaEntrada,
                horaSalida: data.horaSalida.length === 5 ? `${data.horaSalida}:00` : data.horaSalida,
                toleranciaMinutos: Number(data.toleranciaMinutos)
            };

            if (isEditing) {
                await horarioService.update(Number(id), formattedData as UpdateHorarioDto);
            } else {
                await horarioService.create(formattedData as CreateHorarioDto);
            }
            navigate('/horarios');
        } catch (err: any) {
            setError(err.response?.data?.message || 'Error al guardar el horario.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="max-w-2xl mx-auto">
            <div className="mb-6 flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900">
                        {isEditing ? 'Editar Horario' : 'Nuevo Horario'}
                    </h1>
                </div>
                <Link
                    to="/horarios"
                    className="inline-flex items-center text-gray-500 hover:text-gray-700"
                >
                    <ArrowLeft className="h-5 w-5 mr-1" />
                    Volver
                </Link>
            </div>

            {error && <div className="bg-red-50 p-4 rounded-md text-red-700 mb-6">{error}</div>}

            <div className="bg-white shadow rounded-lg p-6">
                <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                    <div>
                        <label className="block text-sm font-medium text-gray-700">Nombre del Horario</label>
                        <input
                            type="text"
                            {...register('nombre', { required: 'El nombre es obligatorio' })}
                            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 border p-2"
                            placeholder="Ej. Turno Matutino"
                        />
                        {errors.nombre && <span className="text-red-500 text-sm">{errors.nombre.message}</span>}
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        <div>
                            <label className="block text-sm font-medium text-gray-700">Hora de Entrada</label>
                            <input
                                type="time"
                                {...register('horaEntrada', { required: 'La hora de entrada es obligatoria' })}
                                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 border p-2"
                            />
                            {errors.horaEntrada && <span className="text-red-500 text-sm">{errors.horaEntrada.message}</span>}
                        </div>

                        <div>
                            <label className="block text-sm font-medium text-gray-700">Hora de Salida</label>
                            <input
                                type="time"
                                {...register('horaSalida', { required: 'La hora de salida es obligatoria' })}
                                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 border p-2"
                            />
                            {errors.horaSalida && <span className="text-red-500 text-sm">{errors.horaSalida.message}</span>}
                        </div>
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700">Tolerancia (minutos)</label>
                        <input
                            type="number"
                            {...register('toleranciaMinutos', {
                                required: 'La tolerancia es obligatoria',
                                min: { value: 0, message: 'Mínimo 0 minutos' },
                                max: { value: 120, message: 'Máximo 120 minutos' }
                            })}
                            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 border p-2"
                        />
                        <p className="mt-1 text-xs text-gray-500">Minutos de tolerancia después de la hora de entrada antes de marcar retardo.</p>
                        {errors.toleranciaMinutos && <span className="text-red-500 text-sm">{errors.toleranciaMinutos.message}</span>}
                    </div>

                    {isEditing && (
                        <div className="flex items-center">
                            <input
                                type="checkbox"
                                {...register('active')}
                                className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                            />
                            <label className="ml-2 block text-sm text-gray-900">
                                Horario Activo
                            </label>
                        </div>
                    )}

                    <div className="flex justify-end pt-4">
                        <button
                            type="submit"
                            disabled={loading}
                            className="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50"
                        >
                            <Save className="-ml-1 mr-2 h-5 w-5" />
                            {loading ? 'Guardando...' : 'Guardar Horario'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
