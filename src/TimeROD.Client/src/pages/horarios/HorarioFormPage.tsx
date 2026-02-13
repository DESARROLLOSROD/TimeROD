import { useState, useEffect } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { ArrowLeft, Save } from 'lucide-react';
import horarioService from '../../services/horarioService';
import type { UpdateHorarioDto } from '../../types/horario';

export default function HorarioFormPage() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const isEditing = !!id;
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const { register, handleSubmit, setValue, formState: { errors } } = useForm<UpdateHorarioDto>({
        defaultValues: {
            nombre: '',
            horaEntrada: '08:00:00', // Default
            horaSalida: '17:00:00',
            toleranciaMinutos: 0,
            activo: true
        }
    });

    useEffect(() => {
        if (isEditing) {
            loadHorario(Number(id));
        }
    }, [id]);

    const loadHorario = async (horarioId: number) => {
        try {
            setLoading(true);
            const data = await horarioService.getById(horarioId);
            setValue('nombre', data.nombre);
            setValue('horaEntrada', data.horaEntrada);
            setValue('horaSalida', data.horaSalida);
            setValue('toleranciaMinutos', data.toleranciaMinutos);
            setValue('activo', data.activo);
        } catch (err) {
            setError('Error al cargar el horario.');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const onSubmit = async (data: UpdateHorarioDto) => {
        setLoading(true);
        setError('');
        try {
            // Ensure seconds are included if input type="time" omits them
            const formatTime = (time: string) => time.length === 5 ? `${time}:00` : time;

            const payload = {
                ...data,
                horaEntrada: formatTime(data.horaEntrada),
                horaSalida: formatTime(data.horaSalida)
            };

            if (isEditing) {
                await horarioService.update(Number(id), payload);
            } else {
                await horarioService.create(payload);
            }
            navigate('/horarios');
        } catch (err: any) {
            setError(err.response?.data?.error || 'Error al guardar el horario.');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="max-w-2xl mx-auto mt-10">
            <div className="mb-6 flex items-center justify-between">
                <div className="flex items-center">
                    <Link to="/horarios" className="text-gray-500 hover:text-gray-700 mr-4">
                        <ArrowLeft className="w-6 h-6" />
                    </Link>
                    <h1 className="text-3xl font-bold text-gray-800">
                        {isEditing ? 'Editar Horario' : 'Nuevo Horario'}
                    </h1>
                </div>
            </div>

            {error && (
                <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
                    {error}
                </div>
            )}

            <div className="bg-white shadow-xl rounded-lg overflow-hidden">
                <div className="p-6 sm:p-10">
                    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                        {/* Nombre */}
                        <div>
                            <label className="block text-sm font-medium text-gray-700">Nombre del Horario</label>
                            <input
                                type="text"
                                {...register('nombre', { required: 'El nombre es requerido' })}
                                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 p-2 border"
                                placeholder="Ej. Turno Matutino"
                            />
                            {errors.nombre && <p className="text-red-500 text-xs mt-1">{errors.nombre.message}</p>}
                        </div>

                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            {/* Hora Entrada */}
                            <div>
                                <label className="block text-sm font-medium text-gray-700">Hora de Entrada</label>
                                <input
                                    type="time"
                                    step="1" // Allow seconds selection if supported
                                    {...register('horaEntrada', { required: 'La hora de entrada es requerida' })}
                                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 p-2 border"
                                />
                                {errors.horaEntrada && <p className="text-red-500 text-xs mt-1">{errors.horaEntrada.message}</p>}
                            </div>

                            {/* Hora Salida */}
                            <div>
                                <label className="block text-sm font-medium text-gray-700">Hora de Salida</label>
                                <input
                                    type="time"
                                    step="1"
                                    {...register('horaSalida', { required: 'La hora de salida es requerida' })}
                                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 p-2 border"
                                />
                                {errors.horaSalida && <p className="text-red-500 text-xs mt-1">{errors.horaSalida.message}</p>}
                            </div>
                        </div>

                        {/* Tolerancia */}
                        <div>
                            <label className="block text-sm font-medium text-gray-700">Tolerancia (minutos)</label>
                            <input
                                type="number"
                                min="0"
                                max="120"
                                {...register('toleranciaMinutos', {
                                    required: 'La tolerancia es requerida',
                                    min: { value: 0, message: 'No puede ser negativa' }
                                })}
                                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 p-2 border"
                            />
                            <p className="text-xs text-gray-500 mt-1">Minutos permitidos después de la hora de entrada antes de considerar retardo.</p>
                            {errors.toleranciaMinutos && <p className="text-red-500 text-xs mt-1">{errors.toleranciaMinutos.message}</p>}
                        </div>

                        {/* Activo (Solo en edición) */}
                        {isEditing && (
                            <div className="flex items-center">
                                <input
                                    type="checkbox"
                                    {...register('activo')}
                                    className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                                />
                                <label className="ml-2 block text-sm text-gray-900">
                                    Horario Activo
                                </label>
                            </div>
                        )}

                        <div className="flex justify-end pt-4">
                            <Link
                                to="/horarios"
                                className="bg-gray-200 text-gray-700 hover:bg-gray-300 font-bold py-2 px-4 rounded mr-2"
                            >
                                Cancelar
                            </Link>
                            <button
                                type="submit"
                                disabled={loading}
                                className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded flex items-center disabled:opacity-50"
                            >
                                <Save className="w-5 h-5 mr-2" />
                                {loading ? 'Guardando...' : 'Guardar Horario'}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
}
