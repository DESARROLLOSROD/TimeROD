# 🗺️ TimeROD - Roadmap de Desarrollo

## Sistema Moderno de Control de Asistencia y Pre-Nómina

---

## ✅ Fase 1: Fundación del Proyecto (COMPLETADO)

### 1.1 Arquitectura Base
- ✅ Estructura de solución .NET 9 con Clean Architecture
  - `TimeROD.Core` - Entidades y lógica de negocio
  - `TimeROD.Infrastructure` - Acceso a datos y Entity Framework
  - `TimeROD.API` - Controllers y endpoints REST
- ✅ Configuración de Entity Framework Core con PostgreSQL
- ✅ Primera migración (`InitialCreate`) con entidades base:
  - Empresa
  - Usuario
  - Area
  - Asistencia

### 1.2 API REST Básica
- ✅ Controller de Empresas con endpoints:
  - `GET /api/empresas` - Listar empresas activas
  - `GET /api/empresas/{id}` - Obtener empresa por ID
  - `POST /api/empresas` - Crear nueva empresa
- ✅ Manejo de errores con logs
- ✅ CORS configurado para frontend React

### 1.3 Deployment en Producción
- ✅ Dockerfile multi-stage optimizado para .NET 9
- ✅ Despliegue en Railway (https://timerod.up.railway.app)
- ✅ PostgreSQL configurado en Railway
- ✅ Parser automático de DATABASE_URL (URI → Npgsql format)
- ✅ Migraciones automáticas en producción
- ✅ SSL Mode configurado
- ✅ CI/CD: GitHub → Railway (auto-deploy)

---

## 🚀 Fase 2: API Completa del Backend (EN PROGRESO - 65% COMPLETADO)

### 2.1 Controllers Restantes ✅ COMPLETADO
- ✅ **UsuariosController** (232 líneas - 7 endpoints)
  - `GET /api/usuarios` - Listar usuarios activos
  - `GET /api/usuarios/{id}` - Obtener usuario
  - `POST /api/usuarios` - Crear usuario (password sin hashear, temporal)
  - `PUT /api/usuarios/{id}` - Actualizar usuario
  - `DELETE /api/usuarios/{id}` - Desactivar usuario (soft delete)
  - `GET /api/usuarios/empresa/{empresaId}` - Usuarios por empresa
  - ✨ Validación de email único
  - ✨ Navegación incluida (Empresa)

- ✅ **AreasController** (195 líneas - 7 endpoints)
  - `GET /api/areas` - Listar áreas activas
  - `GET /api/areas/{id}` - Obtener área con empleados
  - `POST /api/areas` - Crear área
  - `PUT /api/areas/{id}` - Actualizar área
  - `DELETE /api/areas/{id}` - Desactivar área (soft delete)
  - `GET /api/areas/empresa/{empresaId}` - Áreas por empresa
  - ✨ Validación de supervisor asignado
  - ✨ Previene eliminar áreas con empleados activos

- ✅ **EmpleadosController** (307 líneas - 10 endpoints)
  - `GET /api/empleados` - Listar empleados activos
  - `GET /api/empleados/{id}` - Obtener empleado
  - `POST /api/empleados` - Crear empleado
  - `PUT /api/empleados/{id}` - Actualizar empleado
  - `DELETE /api/empleados/{id}` - Desactivar empleado
  - `GET /api/empleados/empresa/{empresaId}` - Empleados por empresa
  - `GET /api/empleados/area/{areaId}` - Empleados por área
  - `GET /api/empleados/numero/{numeroEmpleado}` - Buscar por número
  - ✨ Validación de número de empleado único por empresa
  - ✨ Validación multi-tenant (área y usuario de la misma empresa)

- ✅ **AsistenciasController** (389 líneas - 11 endpoints)
  - `GET /api/asistencias` - Listar asistencias (filtros: empleado, fechas)
  - `GET /api/asistencias/{id}` - Obtener asistencia
  - `POST /api/asistencias/entrada` - **Registrar entrada** ⏰
  - `POST /api/asistencias/salida` - **Registrar salida** ⏰
  - `GET /api/asistencias/empleado/{empleadoId}` - Por empleado
  - `GET /api/asistencias/reporte` - **Reporte con métricas** 📊
  - `PUT /api/asistencias/{id}` - Actualizar asistencia
  - `DELETE /api/asistencias/{id}` - Eliminar asistencia
  - ✨ **Cálculo automático de horas trabajadas**
  - ✨ **DTOs personalizados** (RegistroEntradaDto, RegistroSalidaDto)
  - ✨ **Reportes agregados** (total horas, promedios, llegadas tardías)
  - ✨ Estructura lista para detección de llegadas tardías

- ✅ **EmpresasController (Completado)** (178 líneas - 5 endpoints)
  - ✅ `PUT /api/empresas/{id}` - Actualizar empresa
  - ✅ `DELETE /api/empresas/{id}` - Desactivar empresa (soft delete)
  - ✨ Validación RFC único
  - ✨ Previene eliminar empresas con usuarios/áreas activos

**📊 Total: 31 endpoints | 1,846 líneas de código | 5 controllers completos**

### 2.2 Servicios de Negocio
- [ ] Implementar capa de servicios (Service Layer)
  - `IEmpresaService` / `EmpresaService`
  - `IUsuarioService` / `UsuarioService`
  - `IAsistenciaService` / `AsistenciaService`
  - `IAreaService` / `AreaService`
- [ ] Mover lógica de negocio fuera de controllers
- [ ] Validaciones de negocio (horarios, solapamiento de registros, etc.)

### 2.3 Seguridad y Autenticación
- [ ] Implementar autenticación JWT
- [ ] Endpoint de login: `POST /api/auth/login`
- [ ] Middleware de autorización
- [ ] Roles de usuario (Admin, Supervisor, Empleado)
- [ ] Proteger endpoints según roles

### 2.4 Validación y DTOs
- [ ] Crear DTOs (Data Transfer Objects) para requests/responses
- [ ] FluentValidation para validaciones
- [ ] AutoMapper para mapeo de entidades a DTOs

---

## 📱 Fase 3: Frontend React (FUTURO)

### 3.1 Configuración Inicial
- [ ] Crear proyecto React con Vite
- [ ] Configurar TypeScript
- [ ] Instalar dependencias:
  - React Router (navegación)
  - Axios (HTTP requests)
  - React Hook Form (formularios)
  - TailwindCSS o Material-UI (estilos)
  - React Query (caché y estado servidor)

### 3.2 Estructura de Proyecto
```
src/
├── components/
│   ├── common/        # Componentes reutilizables
│   ├── empresas/      # Componentes de empresas
│   ├── usuarios/      # Componentes de usuarios
│   ├── asistencias/   # Componentes de asistencias
│   └── layout/        # Layout, navbar, sidebar
├── pages/             # Páginas/vistas
├── services/          # API calls
├── hooks/             # Custom hooks
├── context/           # Context API (auth, etc.)
└── utils/             # Utilidades
```

### 3.3 Módulos Principales
- [ ] **Módulo de Autenticación**
  - Login page
  - Manejo de JWT en localStorage
  - ProtectedRoute component
  - Context de autenticación

- [ ] **Módulo de Empresas**
  - Lista de empresas
  - Formulario crear/editar empresa
  - Detalle de empresa

- [ ] **Módulo de Usuarios**
  - Lista de usuarios
  - Formulario crear/editar usuario
  - Asignación de roles
  - Foto de perfil (upload)

- [ ] **Módulo de Áreas**
  - Lista de áreas
  - Formulario crear/editar área
  - Asignación de usuarios a áreas

- [ ] **Módulo de Asistencias**
  - Reloj de entrada/salida (botones grandes)
  - Lista de asistencias del día
  - Historial de asistencias (con filtros)
  - Reportes visuales (gráficos)

### 3.4 Dashboard
- [ ] Dashboard principal con métricas:
  - Total de empleados presentes/ausentes hoy
  - Promedio de horas trabajadas
  - Gráfico de asistencias por semana/mes
  - Lista de llegadas tardías

---

## 🎯 Fase 4: Funcionalidades Avanzadas (FUTURO)

### 4.1 Sistema de Reportes
- [ ] Reporte de asistencias por período
- [ ] Reporte de horas trabajadas por empleado
- [ ] Reporte de llegadas tardías
- [ ] Reporte de ausencias
- [ ] Exportación a PDF
- [ ] Exportación a Excel

### 4.2 Sistema de Horarios
- [ ] Definir horarios de trabajo por área
- [ ] Horarios flexibles vs. horarios fijos
- [ ] Turnos rotativos
- [ ] Detección automática de llegadas tardías
- [ ] Detección de salidas anticipadas

### 4.3 Pre-Nómina
- [ ] Cálculo de horas regulares
- [ ] Cálculo de horas extras
- [ ] Cálculo de bonos por asistencia
- [ ] Deducciones por tardanzas
- [ ] Integración con sistemas de nómina

### 4.4 Notificaciones
- [ ] Notificaciones en tiempo real (SignalR)
- [ ] Email de recordatorio de entrada/salida
- [ ] Alertas de llegadas tardías para supervisores
- [ ] Notificaciones push (PWA)

### 4.5 Geolocalización
- [ ] Captura de ubicación al registrar entrada/salida
- [ ] Validación de ubicación (dentro de zona permitida)
- [ ] Mapa de asistencias

### 4.6 Biometría (Opcional)
- [ ] Integración con dispositivos biométricos
- [ ] API para recibir marcaciones externas
- [ ] Webhook para sincronización

---

## 🔧 Fase 5: Mejoras Técnicas (CONTINUO)

### 5.1 Testing
- [ ] Unit tests para servicios (xUnit)
- [ ] Integration tests para API
- [ ] End-to-end tests para frontend (Playwright)
- [ ] Cobertura de código > 80%

### 5.2 Documentación
- [ ] Swagger/OpenAPI configurado
- [ ] Comentarios XML en código
- [ ] README.md completo
- [ ] Guía de instalación
- [ ] Guía de desarrollo

### 5.3 Performance
- [ ] Caché con Redis
- [ ] Paginación en endpoints
- [ ] Lazy loading en Entity Framework
- [ ] Índices en base de datos
- [ ] CDN para assets del frontend

### 5.4 Monitoring
- [ ] Application Insights o Sentry
- [ ] Health checks en API
- [ ] Logs estructurados (Serilog)
- [ ] Métricas de performance

### 5.5 DevOps
- [ ] GitHub Actions para CI/CD
- [ ] Tests automáticos en PR
- [ ] Deploy automático a staging
- [ ] Backup automático de base de datos

---

## 📊 Métricas de Éxito

### MVP (Mínimo Producto Viable)
- ✅ Backend API desplegado en producción
- ✅ Base de datos PostgreSQL funcionando
- ✅ Al menos 1 endpoint funcional
- [ ] Frontend React básico
- [ ] Login y autenticación
- [ ] Registro de entrada/salida funcional

### v1.0 (Primera Versión Completa)
- [ ] Todos los módulos implementados
- [ ] 5+ empresas utilizando el sistema
- [ ] > 50 usuarios registrados
- [ ] 1000+ registros de asistencia
- [ ] Reportes básicos funcionando

### v2.0 (Versión Avanzada)
- [ ] Sistema de pre-nómina completo
- [ ] Integración con biométricos
- [ ] App móvil (React Native)
- [ ] 20+ empresas
- [ ] > 500 usuarios

---

## 🎯 Próximos Pasos Inmediatos

### Esta Semana ✅ COMPLETADO
1. ✅ Completar controllers restantes (Usuarios, Áreas, Empleados, Asistencias)
2. ✅ Agregar entidad Asistencia y migración
3. ✅ Completar EmpresasController (PUT, DELETE)
4. ✅ Deploy a Railway con todos los endpoints

**Siguiente prioridad:**
1. [ ] Probar todos los endpoints en producción (Railway)
2. [ ] Crear datos de prueba completos (empresa, usuarios, áreas, empleados)
3. [ ] Probar flujo completo de entrada/salida de asistencia

### Próximas 2 Semanas - Backend (Fase 2 Continuación)
**Opción A: Service Layer + Validaciones**
1. [ ] Implementar capa de servicios (Service Layer)
   - IEmpresaService / EmpresaService
   - IUsuarioService / UsuarioService
   - IEmpleadoService / EmpleadoService
   - IAsistenciaService / AsistenciaService
2. [ ] Implementar FluentValidation para DTOs
3. [ ] Crear DTOs personalizados para requests/responses
4. [ ] Mover lógica de negocio a servicios

**Opción B: JWT Authentication (Prioritario para Frontend)**
1. [ ] Implementar autenticación JWT
2. [ ] Crear AuthController con endpoint /api/auth/login
3. [ ] Middleware de autorización
4. [ ] Proteger endpoints según roles (Admin, RH, Supervisor, Empleado)
5. [ ] Implementar hashing de passwords (BCrypt)

### Próximo Mes - Frontend
1. [ ] Iniciar proyecto React con Vite + TypeScript
2. [ ] Crear página de login
3. [ ] Implementar módulo de Empresas (CRUD básico)
4. [ ] Implementar reloj de entrada/salida
5. [ ] Dashboard simple con métricas de hoy
6. [ ] Testing básico (Jest, React Testing Library)

---

## 📝 Notas de Desarrollo

### Stack Tecnológico Actual
- **Backend**: .NET 9, ASP.NET Core, Entity Framework Core
- **Database**: PostgreSQL 16
- **Deployment**: Railway (Docker)
- **CI/CD**: GitHub → Railway auto-deploy
- **Frontend (Futuro)**: React 18, TypeScript, TailwindCSS

### Decisiones de Arquitectura
- Clean Architecture (Core, Infrastructure, API)
- RESTful API design
- JWT para autenticación
- Repository pattern con Entity Framework
- Service layer para lógica de negocio

### Convenciones de Código
- Nombres en español para entidades de negocio
- Nombres en inglés para código técnico
- Async/await en todos los métodos I/O
- Logging con ILogger
- Manejo de errores con try-catch y status codes apropiados

---

## 🤝 Contribución

Para continuar el desarrollo:

1. **Revisar este roadmap** antes de empezar nueva funcionalidad
2. **Crear feature branches** para cada funcionalidad nueva
3. **Commits descriptivos** con mensajes claros
4. **Actualizar este roadmap** al completar tareas
5. **Testing** antes de hacer merge a main

---

## 📞 Contacto

**Desarrollador**: Desarrollos ROD
**Proyecto**: TimeROD
**Repositorio**: https://github.com/DESARROLLOSROD/TimeROD
**API Producción**: https://timerod.up.railway.app

---

**Última actualización**: 2026-02-13
**Versión actual**: 0.2.0-alpha (Fase 2 - Backend API completo 65%)

### 📊 Progreso Actual
- ✅ Fase 1 (Fundación): 100% completado
- 🚧 Fase 2 (Backend API): 65% completado
  - ✅ 31 endpoints funcionando
  - ✅ 5 controllers completos (1,846 líneas)
  - ⏳ Service Layer pendiente
  - ⏳ JWT Authentication pendiente
  - ⏳ DTOs y Validaciones pendiente
- ⏸️ Fase 3 (Frontend): No iniciado
- ⏸️ Fase 4 (Funcionalidades Avanzadas): No iniciado
- 🔄 Fase 5 (Mejoras Técnicas): Continuo
