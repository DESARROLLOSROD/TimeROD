import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { type ReactNode } from 'react';
import { AuthProvider, useAuth } from './context/AuthContext';
import LoginPage from './pages/LoginPage';
import DashboardPage from './pages/DashboardPage';
import MainLayout from './components/layout/MainLayout';
import EmpresasPage from './pages/empresas/EmpresasPage';
import EmpresaFormPage from './pages/empresas/EmpresaFormPage';
import UsuariosPage from './pages/usuarios/UsuariosPage';
import UsuarioFormPage from './pages/usuarios/UsuarioFormPage';
import AreasPage from './pages/areas/AreasPage';
import AreaFormPage from './pages/areas/AreaFormPage';
import EmpleadosPage from './pages/empleados/EmpleadosPage';
import EmpleadoFormPage from './pages/empleados/EmpleadoFormPage';
import AsistenciasPage from './pages/asistencias/AsistenciasPage';
import RelojChecadorPage from './pages/asistencias/RelojChecadorPage';

const ProtectedRoute = ({ children }: { children: ReactNode }) => {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return <div className="flex items-center justify-center h-screen">Cargando...</div>;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return children;
};

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <div className="min-h-screen bg-gray-100 text-gray-900 font-sans">
          <Routes>
            <Route path="/login" element={<LoginPage />} />

            {/* Protected Routes wrapped in MainLayout */}
            <Route element={
              <ProtectedRoute>
                <MainLayout />
              </ProtectedRoute>
            }>
              <Route path="/dashboard" element={<DashboardPage />} />

              {/* Empresas Routes */}
              <Route path="/empresas" element={<EmpresasPage />} />
              <Route path="/empresas/nueva" element={<EmpresaFormPage />} />
              <Route path="/empresas/editar/:id" element={<EmpresaFormPage />} />

              {/* Usuarios Routes */}
              <Route path="/usuarios" element={<UsuariosPage />} />
              <Route path="/usuarios/nuevo" element={<UsuarioFormPage />} />
              <Route path="/usuarios/editar/:id" element={<UsuarioFormPage />} />

              {/* Areas Routes */}
              <Route path="/areas" element={<AreasPage />} />
              <Route path="/areas/nueva" element={<AreaFormPage />} />
              <Route path="/areas/editar/:id" element={<AreaFormPage />} />

              {/* Empleados Routes */}
              <Route path="/empleados" element={<EmpleadosPage />} />
              <Route path="/empleados/nueva" element={<EmpleadoFormPage />} />
              <Route path="/empleados/editar/:id" element={<EmpleadoFormPage />} />

              {/* Asistencias Routes */}
              <Route path="/asistencias" element={<AsistenciasPage />} />
              <Route path="/reloj-checador" element={<RelojChecadorPage />} />

              {/* Future routes will go here: /usuarios, etc. */}
            </Route>

            <Route path="/" element={<Navigate to="/dashboard" replace />} />
            <Route path="*" element={<div className="p-8 text-center text-red-500">404 - Not Found</div>} />
          </Routes>
        </div>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
