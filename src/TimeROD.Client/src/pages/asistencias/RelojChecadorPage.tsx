import { useState, useEffect } from 'react';
import { Clock, LogIn, LogOut, MapPin } from 'lucide-react';
import asistenciaService from '../../services/asistenciaService';
import empleadoService from '../../services/empleadoService';
import type { EmpleadoDto } from '../../types/empleado';

export default function RelojChecadorPage() {
    const [currentTime, setCurrentTime] = useState(new Date());
    const [empleadoId, setEmpleadoId] = useState('');
    const [empleados, setEmpleados] = useState<EmpleadoDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState<{ type: 'success' | 'error', text: string } | null>(null);
    const [location, setLocation] = useState<{ lat: number, lon: number } | null>(null);

    useEffect(() => {
        const timer = setInterval(() => setCurrentTime(new Date()), 1000);
        loadEmpleados();
        getLocation();
        return () => clearInterval(timer);
    }, []);

    const loadEmpleados = async () => {
        try {
            const data = await empleadoService.getAll();
            setEmpleados(data);
        } catch (err) {
            console.error('Error loading employees', err);
        }
    };

    const getLocation = () => {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                (position) => {
                    setLocation({
                        lat: position.coords.latitude,
                        lon: position.coords.longitude
                    });
                },
                (error) => {
                    console.warn("Geolocation error:", error);
                }
            );
        }
    };

    const handleRegistro = async (tipo: 'entrada' | 'salida') => {
        if (!empleadoId) {
            setMessage({ type: 'error', text: 'Por favor seleccione un empleado.' });
            return;
        }

        setLoading(true);
        setMessage(null);

        try {
            const now = new Date();
            const fecha = now.toISOString().split('T')[0];
            const hora = now.toTimeString().split(' ')[0]; // HH:mm:ss

            const baseDto = {
                empleadoId: Number(empleadoId),
                fecha: fecha,
                hora: hora,
                latitud: location?.lat,
                longitud: location?.lon
            };

            if (tipo === 'entrada') {
                await asistenciaService.registrarEntrada(baseDto);
                setMessage({ type: 'success', text: `Entrada registrada correctamente a las ${hora}` });
            } else {
                await asistenciaService.registrarSalida(baseDto);
                setMessage({ type: 'success', text: `Salida registrada correctamente a las ${hora}` });
            }

            // Clear selection after success? Maybe keep it for rapid testing.
        } catch (err: any) {
            const errorMsg = err.response?.data?.error || err.response?.data?.message || 'Error al registrar asistencia.';
            setMessage({ type: 'error', text: errorMsg });
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="max-w-md mx-auto mt-10">
            <div className="bg-white shadow-xl rounded-lg overflow-hidden">
                <div className="bg-blue-600 px-6 py-4 text-center">
                    <h2 className="text-2xl font-bold text-white flex items-center justify-center">
                        <Clock className="w-8 h-8 mr-2" />
                        Reloj Checador
                    </h2>
                    <p className="text-blue-100 mt-1">Registre su entrada o salida</p>
                </div>

                <div className="p-8">
                    <div className="text-center mb-8">
                        <div className="text-5xl font-mono text-gray-800 font-bold tracking-wider">
                            {currentTime.toLocaleTimeString([], { hour12: false })}
                        </div>
                        <div className="text-gray-500 mt-2">
                            {currentTime.toLocaleDateString([], { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}
                        </div>
                    </div>

                    <div className="space-y-6">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">Seleccione Empleado</label>
                            <select
                                value={empleadoId}
                                onChange={(e) => setEmpleadoId(e.target.value)}
                                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 p-2 border"
                            >
                                <option value="">-- Seleccionar --</option>
                                {empleados.map(e => (
                                    <option key={e.id} value={e.id}>{e.nombre} {e.apellidos} ({e.numeroEmpleado})</option>
                                ))}
                            </select>
                        </div>

                        {message && (
                            <div className={`p-4 rounded-md ${message.type === 'success' ? 'bg-green-50 text-green-700' : 'bg-red-50 text-red-700'}`}>
                                {message.text}
                            </div>
                        )}

                        <div className="grid grid-cols-2 gap-4">
                            <button
                                onClick={() => handleRegistro('entrada')}
                                disabled={loading}
                                className="flex flex-col items-center justify-center p-4 bg-green-50 rounded-lg border-2 border-green-200 hover:bg-green-100 hover:border-green-300 transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
                            >
                                <LogIn className="w-8 h-8 text-green-600 mb-2" />
                                <span className="font-bold text-green-700">ENTRADA</span>
                            </button>

                            <button
                                onClick={() => handleRegistro('salida')}
                                disabled={loading}
                                className="flex flex-col items-center justify-center p-4 bg-red-50 rounded-lg border-2 border-red-200 hover:bg-red-100 hover:border-red-300 transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
                            >
                                <LogOut className="w-8 h-8 text-red-600 mb-2" />
                                <span className="font-bold text-red-700">SALIDA</span>
                            </button>
                        </div>

                        {location && (
                            <div className="text-center text-xs text-gray-400 flex items-center justify-center mt-4">
                                <MapPin className="w-3 h-3 mr-1" />
                                <span>Ubicación detectada</span>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}
