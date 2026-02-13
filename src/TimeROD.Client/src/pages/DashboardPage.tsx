import { useAuth } from '../context/AuthContext';
import { Users, Clock, Building2 } from 'lucide-react';

export default function DashboardPage() {
    const { user } = useAuth();

    const stats = [
        { name: 'Mi Empresa', value: user?.empresaNombre || 'N/A', icon: Building2, color: 'bg-blue-500' },
        { name: 'Estado', value: user?.activo ? 'Activo' : 'Inactivo', icon: Users, color: 'bg-green-500' },
        { name: 'Rol', value: user?.rol || 'N/A', icon: Clock, color: 'bg-purple-500' },
    ];

    return (
        <div>
            <h1 className="text-2xl font-bold text-gray-900 mb-6">Dashboard</h1>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                {stats.map((stat) => (
                    <div key={stat.name} className="bg-white overflow-hidden shadow rounded-lg">
                        <div className="p-5">
                            <div className="flex items-center">
                                <div className="flex-shrink-0">
                                    <div className={`rounded-md p-3 ${stat.color}`}>
                                        <stat.icon className="h-6 w-6 text-white" aria-hidden="true" />
                                    </div>
                                </div>
                                <div className="ml-5 w-0 flex-1">
                                    <dl>
                                        <dt className="text-sm font-medium text-gray-500 truncate">{stat.name}</dt>
                                        <dd>
                                            <div className="text-lg font-medium text-gray-900">{stat.value}</div>
                                        </dd>
                                    </dl>
                                </div>
                            </div>
                        </div>
                    </div>
                ))}
            </div>

            <div className="mt-8 bg-white shadow rounded-lg p-6">
                <h2 className="text-lg font-medium text-gray-900 mb-4">Bienvenido al Sistema TimeROD</h2>
                <p className="text-gray-600">
                    Selecciona una opción del menú lateral para comenzar a gestionar tu empresa, empleados o registros de asistencia.
                </p>
            </div>
        </div>
    );
}
