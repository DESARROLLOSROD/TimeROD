import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { X, Save } from 'lucide-react';
import type { AsistenciaDto, UpdateAsistenciaDto } from '../../types/asistencia';

interface AsistenciaEditModalProps {
    asistencia: AsistenciaDto;
    isOpen: boolean;
    onClose: () => void;
    onSave: (id: number, data: UpdateAsistenciaDto) => Promise<void>;
}

export default function AsistenciaEditModal({ asistencia, isOpen, onClose, onSave }: AsistenciaEditModalProps) {
    const { register, handleSubmit, reset } = useForm<UpdateAsistenciaDto>();

    useEffect(() => {
        if (asistencia) {
            reset({
                horaEntrada: asistencia.horaEntrada ? new Date(asistencia.horaEntrada).toLocaleTimeString('en-GB') : '',
                horaSalida: asistencia.horaSalida ? new Date(asistencia.horaSalida).toLocaleTimeString('en-GB') : '',
                tipo: asistencia.tipo,
                notas: asistencia.observaciones || '', // Map observaciones to notas
                aprobado: true, // Default to approved on edit?
                llegadaTardia: asistencia.llegadaTardia,
                minutosRetraso: asistencia.minutosRetraso || 0,
                salidaAnticipada: asistencia.salidaAnticipada,
                minutosAnticipados: asistencia.minutosAnticipados || 0,
            });
        }
    }, [asistencia, reset]);

    if (!isOpen) return null;

    const onSubmit = (data: UpdateAsistenciaDto) => {
        // Convert time strings back to full DateTime strings if needed by backend, 
        // OR backend handles mapping. The backend expects DateTime?.
        // We need to combine the original date with the new time.

        const datePart = asistencia.fecha.split('T')[0];

        const formattedData: UpdateAsistenciaDto = {
            ...data,
            horaEntrada: data.horaEntrada ? `${datePart}T${data.horaEntrada}` : undefined,
            horaSalida: data.horaSalida ? `${datePart}T${data.horaSalida}` : undefined,
            minutosRetraso: data.minutosRetraso ? Number(data.minutosRetraso) : undefined,
            minutosAnticipados: data.minutosAnticipados ? Number(data.minutosAnticipados) : undefined,
        };

        onSave(asistencia.id, formattedData);
    };

    return (
        <div className="fixed inset-0 z-50 overflow-y-auto">
            <div className="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
                <div className="fixed inset-0 transition-opacity" aria-hidden="true">
                    <div className="absolute inset-0 bg-gray-500 opacity-75" onClick={onClose}></div>
                </div>

                <span className="hidden sm:inline-block sm:align-middle sm:h-screen" aria-hidden="true">&#8203;</span>

                <div className="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
                    <div className="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
                        <div className="flex justify-between items-center mb-4">
                            <h3 className="text-lg leading-6 font-medium text-gray-900">
                                Editar Asistencia: {asistencia.empleadoNombreCompleto}
                            </h3>
                            <button onClick={onClose} className="text-gray-400 hover:text-gray-500">
                                <X className="h-6 w-6" />
                            </button>
                        </div>

                        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                            <div className="grid grid-cols-2 gap-4">
                                <div>
                                    <label className="block text-sm font-medium text-gray-700">Hora Entrada</label>
                                    <input
                                        type="time"
                                        step="1"
                                        {...register('horaEntrada')}
                                        className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                                    />
                                </div>
                                <div>
                                    <label className="block text-sm font-medium text-gray-700">Hora Salida</label>
                                    <input
                                        type="time"
                                        step="1"
                                        {...register('horaSalida')}
                                        className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                                    />
                                </div>
                            </div>

                            <div>
                                <label className="block text-sm font-medium text-gray-700">Tipo</label>
                                <select
                                    {...register('tipo')}
                                    className="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm rounded-md"
                                >
                                    <option value="Normal">Normal</option>
                                    <option value="Falta">Falta</option>
                                    <option value="Retardo">Retardo</option>
                                    <option value="Vacaciones">Vacaciones</option>
                                    <option value="Incapacidad">Incapacidad</option>
                                    <option value="Permiso">Permiso</option>
                                </select>
                            </div>

                            <div>
                                <label className="block text-sm font-medium text-gray-700">Notas / Justificación</label>
                                <textarea
                                    {...register('notas')}
                                    rows={3}
                                    className="mt-1 block w-full shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm border border-gray-300 rounded-md"
                                />
                            </div>

                            <div className="space-y-2 border-t pt-4">
                                <div className="flex items-center justify-between">
                                    <div className="flex items-center">
                                        <input
                                            id="llegadaTardia"
                                            type="checkbox"
                                            {...register('llegadaTardia')}
                                            className="h-4 w-4 text-red-600 focus:ring-red-500 border-gray-300 rounded"
                                        />
                                        <label htmlFor="llegadaTardia" className="ml-2 block text-sm text-gray-900">
                                            Llegada Tarde
                                        </label>
                                    </div>
                                    <div className="w-24">
                                        <input
                                            type="number"
                                            placeholder="Min"
                                            {...register('minutosRetraso')}
                                            className="block w-full border border-gray-300 rounded-md shadow-sm py-1 px-2 text-sm"
                                        />
                                    </div>
                                </div>

                                <div className="flex items-center justify-between">
                                    <div className="flex items-center">
                                        <input
                                            id="salidaAnticipada"
                                            type="checkbox"
                                            {...register('salidaAnticipada')}
                                            className="h-4 w-4 text-orange-600 focus:ring-orange-500 border-gray-300 rounded"
                                        />
                                        <label htmlFor="salidaAnticipada" className="ml-2 block text-sm text-gray-900">
                                            Salida Anticipada
                                        </label>
                                    </div>
                                    <div className="w-24">
                                        <input
                                            type="number"
                                            placeholder="Min"
                                            {...register('minutosAnticipados')}
                                            className="block w-full border border-gray-300 rounded-md shadow-sm py-1 px-2 text-sm"
                                        />
                                    </div>
                                </div>
                            </div>

                            <div className="mt-5 sm:mt-6 sm:grid sm:grid-cols-2 sm:gap-3 sm:grid-flow-row-dense">
                                <button
                                    type="submit"
                                    className="w-full inline-flex justify-center rounded-md border border-transparent shadow-sm px-4 py-2 bg-blue-600 text-base font-medium text-white hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 sm:col-start-2 sm:text-sm"
                                >
                                    <Save className="h-5 w-5 mr-2" />
                                    Guardar Cambios
                                </button>
                                <button
                                    type="button"
                                    onClick={onClose}
                                    className="mt-3 w-full inline-flex justify-center rounded-md border border-gray-300 shadow-sm px-4 py-2 bg-white text-base font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 sm:mt-0 sm:col-start-1 sm:text-sm"
                                >
                                    Cancelar
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
}
