import { Menu, User, Bell } from 'lucide-react';
import { useAuth } from '../../context/AuthContext';

interface NavbarProps {
    toggleSidebar: () => void;
}

export default function Navbar({ toggleSidebar }: NavbarProps) {
    const { user } = useAuth();

    return (
        <header className="bg-white border-b border-gray-200 h-16 flex items-center justify-between px-4 sm:px-6 lg:px-8">
            <div className="flex items-center">
                <button
                    onClick={toggleSidebar}
                    className="p-2 -ml-2 mr-2 text-gray-500 rounded-md lg:hidden hover:text-gray-900 hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-blue-500"
                >
                    <Menu className="h-6 w-6" />
                </button>
                <div className="hidden lg:block lg:w-px lg:h-6 lg:bg-gray-200 lg:mx-4"></div>
            </div>

            <div className="flex items-center space-x-4">
                <button className="p-1 text-gray-400 rounded-full hover:text-gray-500 focus:outline-none">
                    <span className="sr-only">Notificaciones</span>
                    <Bell className="h-6 w-6" />
                </button>

                <div className="relative flex items-center ml-3">
                    <div className="flex flex-col text-right mr-3 hidden sm:block">
                        <span className="text-sm font-medium text-gray-700">{user?.nombreCompleto}</span>
                        <span className="text-xs text-gray-500">{user?.rol}</span>
                    </div>
                    <div className="h-8 w-8 rounded-full bg-blue-100 flex items-center justify-center border border-blue-200">
                        <User className="h-5 w-5 text-blue-600" />
                    </div>
                </div>
            </div>
        </header>
    );
}
