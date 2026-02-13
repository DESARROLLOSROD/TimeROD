import { NavLink } from 'react-router-dom';
import {
    LayoutDashboard,
    Users,
    Building2,
    MapPin,
    Clock,
    LogOut,
    Menu
} from 'lucide-react';
import { useAuth } from '../../context/AuthContext';
import clsx from 'clsx';

interface SidebarProps {
    isOpen: boolean;
    setIsOpen: (isOpen: boolean) => void;
}

export default function Sidebar({ isOpen, setIsOpen }: SidebarProps) {
    const { logout } = useAuth();

    const navigation = [
        { name: 'Dashboard', href: '/dashboard', icon: LayoutDashboard },
        { name: 'Empresas', href: '/empresas', icon: Building2 },
        { name: 'Áreas', href: '/areas', icon: MapPin },
        { name: 'Usuarios', href: '/usuarios', icon: Users },
        { name: 'Empleados', href: '/empleados', icon: Users }, // Reuse Users icon or find a specific one
        { name: 'Horarios', href: '/horarios', icon: Clock },
        { name: 'Asistencias', href: '/asistencias', icon: Clock },
        { name: 'Reloj Checador', href: '/reloj-checador', icon: Clock },
    ];

    return (
        <>
            {/* Mobile overlay */}
            <div
                className={clsx(
                    "fixed inset-0 z-20 bg-gray-900/50 lg:hidden transition-opacity duration-300",
                    isOpen ? "opacity-100" : "opacity-0 pointer-events-none"
                )}
                onClick={() => setIsOpen(false)}
            />

            {/* Sidebar component */}
            <div
                className={clsx(
                    "fixed inset-y-0 left-0 z-30 w-64 bg-white border-r border-gray-200 transform transition-transform duration-300 lg:translate-x-0 lg:static lg:inset-auto",
                    isOpen ? "translate-x-0" : "-translate-x-full"
                )}
            >
                <div className="flex items-center justify-between h-16 px-6 border-b border-gray-200 bg-blue-600">
                    <h1 className="text-xl font-bold text-white">TimeROD</h1>
                    <button
                        onClick={() => setIsOpen(false)}
                        className="lg:hidden text-white focus:outline-none"
                    >
                        <Menu className="h-6 w-6" />
                    </button>
                </div>

                <nav className="p-4 space-y-1 overflow-y-auto h-[calc(100vh-4rem)] flex flex-col justify-between">
                    <ul className="space-y-1">
                        {navigation.map((item) => (
                            <li key={item.name}>
                                <NavLink
                                    to={item.href}
                                    className={({ isActive }) =>
                                        clsx(
                                            "flex items-center px-4 py-3 text-sm font-medium rounded-lg transition-colors duration-150",
                                            isActive
                                                ? "bg-blue-50 text-blue-700"
                                                : "text-gray-700 hover:bg-gray-100 hover:text-gray-900"
                                        )
                                    }
                                >
                                    <item.icon className="w-5 h-5 mr-3" />
                                    {item.name}
                                </NavLink>
                            </li>
                        ))}
                    </ul>

                    <div className="pt-4 border-t border-gray-200">
                        <button
                            onClick={logout}
                            className="flex w-full items-center px-4 py-3 text-sm font-medium text-red-600 rounded-lg hover:bg-red-50 transition-colors duration-150"
                        >
                            <LogOut className="w-5 h-5 mr-3" />
                            Cerrar Sesión
                        </button>
                    </div>
                </nav>
            </div>
        </>
    );
}
