# TimeROD - Sistema de Control de Asistencia y Pre-Nómina

Sistema web moderno para control de asistencia, comedor y pre-nómina, orientado a empresas con proyectos remotos (minas, construcción, campo).

## Estructura del Proyecto

```
TimeROD/
├── src/
│   ├── TimeROD.API/            # API REST (.NET 9)
│   │   ├── Controllers/        # Endpoints HTTP
│   │   ├── Program.cs          # Configuración de la aplicación
│   │   └── appsettings.json    # Configuración (BD, etc.)
│   │
│   ├── TimeROD.Core/           # Lógica de negocio y entidades
│   │   ├── Entities/           # Modelos de datos (Empresa, Empleado, etc.)
│   │   ├── Interfaces/         # Contratos de servicios
│   │   └── Services/           # Reglas de negocio (cálculo LFT, etc.)
│   │
│   └── TimeROD.Infrastructure/ # Acceso a datos
│       ├── Data/               # DbContext y Migrations
│       ├── Repositories/       # Acceso a BD
│       └── Services/           # Servicios externos (Email, Storage)
│
├── tests/                      # Tests (futuro)
├── docs/                       # Documentación
└── TimeROD.sln                 # Solución .NET

```

## Stack Técnico

- **Backend:** .NET 9 (Web API)
- **Base de datos:** PostgreSQL (Supabase)
- **ORM:** Entity Framework Core
- **Frontend:** React + TypeScript (próximamente)
- **Hosting:** Railway

## Requisitos

- .NET 9 SDK
- Node.js 22+
- PostgreSQL 15+ (o cuenta Supabase)
- Visual Studio Code

## Comandos útiles

### Backend

```bash
# Restaurar dependencias
dotnet restore

# Compilar
dotnet build

# Ejecutar API (http://localhost:5000)
cd src/TimeROD.API
dotnet run

# Crear migration
dotnet ef migrations add NombreMigracion --project src/TimeROD.Infrastructure --startup-project src/TimeROD.API

# Aplicar migrations
dotnet ef database update --project src/TimeROD.Infrastructure --startup-project src/TimeROD.API
```

### Frontend (próximamente)

```bash
cd src/timerod-web
npm install
npm run dev
```

## Estado actual

✅ Estructura de proyecto creada
⏳ Modelos de datos (próximo)
⏳ API básica (próximo)
⏳ Frontend (próximo)

## Documentación

Ver [PRD completo](prd_sistema_moderno_de_control_de_asistencia_y_pre_nomina.md)

---

**DESARROLLOS ROD** - Durango, México
