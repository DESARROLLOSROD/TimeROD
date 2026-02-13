import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { ArrowLeft, Save } from 'lucide-react';
import empleadoService from '../../services/empleadoService';
import empresaService from '../../services/empresaService';
import areaService from '../../services/areaService';
import horarioService from '../../services/horarioService';
import type { CreateEmpleadoDto, UpdateEmpleadoDto } from '../../types/empleado';
import type { EmpresaDto } from '../../types/empresa';
import type { AreaDto } from '../../types/area';
import type { HorarioDto } from '../../types/horario';

export default function EmpleadoFormPage() {
    const { id } = useParams<{ id: string }>();
    const isEditing = !!id;
    const navigate = useNavigate();
    const { register, handleSubmit, formState: { errors }, setValue, watch } = useForm<CreateEmpleadoDto & UpdateEmpleadoDto>();
    const [loading, setLoading] = useState(false);
    const [empresas, setEmpresas] = useState<EmpresaDto[]>([]);
    const [allAreas, setAllAreas] = useState<AreaDto[]>([]);
    const [filteredAreas, setFilteredAreas] = useState<AreaDto[]>([]);
    const [horarios, setHorarios] = useState<HorarioDto[]>([]);
    const [error, setError] = useState('');

    const selectedEmpresaId = watch('empresaId');

    useEffect(() => {
        loadDependencies();
        if (isEditing) {
            loadEmpleado(Number(id));
        }
    }, [id]);

    useEffect(() => {
        // Filter areas when empresa selection changes
        if (selectedEmpresaId && allAreas.length > 0) {
            const filtered = allAreas.filter(a => a.empresaId === Number(selectedEmpresaId));
            setFilteredAreas(filtered);
        } else {
            setFilteredAreas([]);
        }
    }, [selectedEmpresaId, allAreas]);

    const loadDependencies = async () => {
        try {
            const [empresasData, areasData, horariosData] = await Promise.all([
                empresaService.getAll(),
                areaService.getAll(),
                horarioService.getAll()
            ]);
            setEmpresas(empresasData);
            setAllAreas(areasData);
            setHorarios(horariosData);
        } catch (err) {
            console.error('Error loading dependencies', err);
        }
    };

    const loadEmpleado = async (empleadoId: number) => {
        try {
            setLoading(true);
            const data = await empleadoService.getById(empleadoId);
            // Pre-select values
            setValue('numeroEmpleado', data.numeroEmpleado);
            setValue('nombre', data.nombre);
            setValue('apellidos', data.apellidos);
            setValue('fechaIngreso', data.fechaIngreso.split('T')[0]); // Extract YYYY-MM-DD
            setValue('salarioDiario', data.salarioDiario);
            setValue('empresaId', data.empresaId);
            setValue('areaId', data.areaId);
            setValue('puesto', data.puesto);
            setValue('activo', data.activo);
            setValue('horarioId', data.horarioId);
            // Handling optional fields
            if (data.idBiometrico) setValue('idBiometrico', data.idBiometrico);
        } catch (err) {
            setError('Error al cargar el empleado.');
        } finally {
            setLoading(false);
        }
    };

    const onSubmit = async (data: any) => {
        setLoading(true);
        setError('');
        try {
            // Conversions
            data.empresaId = Number(data.empresaId);
            data.areaId = Number(data.areaId);
            data.salarioDiario = Number(data.salarioDiario);
            if (data.idBiometrico) data.idBiometrico = Number(data.idBiometrico);
            if (data.horarioId) data.horarioId = Number(data.horarioId);
            else data.horarioId = null;

            if (isEditing) {
                await empleadoService.update(Number(id), data as UpdateEmpleadoDto);
            } else {
                await empleadoService.create(data as CreateEmpleadoDto);
            }
            navigate('/empleados');
        } catch (err) {
            setError('Error al guardar el empleado. Verifique los datos.');
        } finally {
            setLoading(false);
        }
    };

    if (loading && isEditing) return <div className="text-center py-10">Cargando...</div>;

    return (
        <div className="max-w-4xl mx-auto">
            <div className="mb-6 flex items-center justify-between">
                <div className="flex items-center">
                    <Link to="/empleados" className="mr-4 text-gray-500 hover:text-gray-700">
                        <ArrowLeft className="h-6 w-6" />
                    </Link>
                    <h1 className="text-2xl font-bold text-gray-900">
                        {isEditing ? 'Editar Empleado' : 'Nuevo Empleado'}
                    </h1>
                </div>
            </div>

            {error && <div className="mb-4 bg-red-50 p-4 rounded-md text-red-700">{error}</div>}

            <div className="bg-white shadow rounded-lg p-6">
                <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        {/* Datos Personales */}
                        <div className="space-y-6">
                            <h3 className="text-lg font-medium text-gray-900 border-b pb-2">Datos Personales</h3>

                            <div>
                                <label className="block text-sm font-medium text-gray-700">Nombre</label>
                                <input
                                    type="text"
                                    {...register('nombre', { required: 'Campo obligatorio' })}
                                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                                />
                                {errors.nombre && <p className="mt-1 text-sm text-red-600">{errors.nombre.message}</p>}
                            </div>

                            <div>
                                <label className="block text-sm font-medium text-gray-700">Apellidos</label>
                                <input
                                    type="text"
                                    {...register('apellidos', { required: 'Campo obligatorio' })}
                                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                                />
                                {errors.apellidos && <p className="mt-1 text-sm text-red-600">{errors.apellidos.message}</p>}
                            </div>
                        </div>

                        {/* Datos Laborales */}
                        <div className="space-y-6">
                            <h3 className="text-lg font-medium text-gray-900 border-b pb-2">Datos Laborales</h3>

                            <div>
                                <label className="block text-sm font-medium text-gray-700">Número de Empleado</label>
                                <input
                                    type="text"
                                    {...register('numeroEmpleado', { required: 'Campo obligatorio' })}
                                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                                />
                                {errors.numeroEmpleado && <p className="mt-1 text-sm text-red-600">{errors.numeroEmpleado.message}</p>}
                            </div>

                            <div>
                                <label className="block text-sm font-medium text-gray-700">Empresa</label>
                                <select
                                    {...register('empresaId', { required: 'Seleccione una empresa' })}
                                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                                >
                                    <option value="">Seleccione...</option>
                                    {empresas.map((e) => (
                                        <option key={e.id} value={e.id}>{e.nombre}</option>
                                    ))}
                                </select>
                                {errors.empresaId && <p className="mt-1 text-sm text-red-600">{errors.empresaId.message}</p>}
                            </div>

                            <div>
                                <label className="block text-sm font-medium text-gray-700">Área</label>
                                <select
                                    {...register('areaId', { required: 'Seleccione un área' })}
                                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                                    disabled={!selectedEmpresaId || filteredAreas.length === 0}
                                >
                                    <option value="">Seleccione...</option>
                                    {filteredAreas.map((a) => (
                                        <option key={a.id} value={a.id}>{a.nombre}</option>
                                    ))}
                                </select>
                                {errors.areaId && <p className="mt-1 text-sm text-red-600">{errors.areaId.message}</p>}
                            </div>

                            <div>
                                <label className="block text-sm font-medium text-gray-700">Puesto</label>
                                <input
                                    type="text"
                                    {...register('puesto')}
                                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                                />
                            </div>

                            <div>
                                <label className="block text-sm font-medium text-gray-700">Horario Personalizado (Opcional)</label>
                                <select
                                    {...register('horarioId')}
                                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                                >
                                    <option value="">-- Usar horario del área --</option>
                                    {horarios.map((h) => (
                                        <option key={h.id} value={h.id}>
                                            {h.nombre} ({h.horaEntrada} - {h.horaSalida})
                                        </option>
                                    ))}
                                </select>
                                <p className="text-xs text-gray-500 mt-1">
                                    Sobreescribe el horario asignado al área.
                                </p>
                            </div>

                            <div className="grid grid-cols-2 gap-4">
                                <div>
                                    <label className="block text-sm font-medium text-gray-700">Fecha Ingreso</label>
                                    <input
                                        type="date"
                                        {...register('fechaIngreso', { required: 'Obligatorio' })}
                                        className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                                    />
                                    {errors.fechaIngreso && <p className="mt-1 text-sm text-red-600">{errors.fechaIngreso.message}</p>}
                                </div>
                                <div>
                                    <label className="block text-sm font-medium text-gray-700">Salario Diario</label>
                                    <input
                                        type="number"
                                        step="0.01"
                                        {...register('salarioDiario', { required: 'Obligatorio', min: 0 })}
                                        className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                                    />
                                    {errors.salarioDiario && <p className="mt-1 text-sm text-red-600">{errors.salarioDiario.message}</p>}
                                </div>
                            </div>

                            <div>
                                <label className="block text-sm font-medium text-gray-700">ID Biométrico (Opcional)</label>
                                <input
                                    type="number"
                                    {...register('idBiometrico')}
                                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border"
                                />
                            </div>

                            <div className="flex items-center pt-2">
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

                        </div>
                    </div>

                    <div className="flex justify-end pt-4 border-t">
                        <Link
                            to="/empleados"
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
