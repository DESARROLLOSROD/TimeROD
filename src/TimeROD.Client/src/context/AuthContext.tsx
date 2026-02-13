import React, { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import type { UsuarioDto, LoginRequestDto } from '../types/auth'; // Adjust import
import authService from '../services/authService';

interface AuthContextType {
    user: UsuarioDto | null;
    isAuthenticated: boolean;
    login: (credentials: LoginRequestDto) => Promise<void>;
    logout: () => void;
    isLoading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [user, setUser] = useState<UsuarioDto | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        // Hydrate state from localStorage
        const storedUser = localStorage.getItem('user');
        const token = localStorage.getItem('token'); // Should validate expiry
        if (storedUser && token) {
            setUser(JSON.parse(storedUser));
        }
        setIsLoading(false);
    }, []);

    const login = async (credentials: LoginRequestDto) => {
        setIsLoading(true);
        try {
            const data = await authService.login(credentials);
            localStorage.setItem('token', data.token);
            localStorage.setItem('user', JSON.stringify(data.usuario));
            setUser(data.usuario);
        } finally {
            setIsLoading(false);
        }
    };

    const logout = () => {
        authService.logout();
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{
            user,
            isAuthenticated: !!user,
            login,
            logout,
            isLoading
        }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};
