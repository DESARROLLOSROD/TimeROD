import api from './api';
import type { LoginRequestDto, LoginResponseDto } from '../types/auth'; // We'll create types next

const authService = {
    login: async (credentials: LoginRequestDto): Promise<LoginResponseDto> => {
        const response = await api.post<LoginResponseDto>('/auth/login', credentials);
        return response.data;
    },

    logout: () => {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
    },

    getCurrentUser: () => {
        const userStr = localStorage.getItem('user');
        if (userStr) return JSON.parse(userStr);
        return null;
    },

    isAuthenticated: () => {
        const token = localStorage.getItem('token');
        // Basic check, ideally verify token expiry
        return !!token;
    }
};

export default authService;
