using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TimeROD.Core.DTOs;
using TimeROD.Core.Entities;
using TimeROD.Core.Interfaces;
using TimeROD.Infrastructure.Data;

namespace TimeROD.Infrastructure.Services;

public class AsistenciaService : IAsistenciaService
{
    private readonly TimeRODDbContext _context;
    private readonly ILogger<AsistenciaService> _logger;

    public AsistenciaService(TimeRODDbContext context, ILogger<AsistenciaService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<AsistenciaDto>> GetAllAsync(int? empleadoId = null, DateTime? fechaInicio = null, DateTime? fechaFin = null)
    {
        var query = _context.Asistencias
            .Include(a => a.Empleado)
                .ThenInclude(e => e.Area)
            .Include(a => a.Empleado)
                .ThenInclude(e => e.Empresa)
            .AsQueryable();

        if (empleadoId.HasValue)
        {
            query = query.Where(a => a.EmpleadoId == empleadoId.Value);
        }

        if (fechaInicio.HasValue)
        {
            query = query.Where(a => a.Fecha >= fechaInicio.Value);
        }

        if (fechaFin.HasValue)
        {
            query = query.Where(a => a.Fecha <= fechaFin.Value);
        }

        var asistencias = await query
            .OrderByDescending(a => a.Fecha)
            .ThenByDescending(a => a.HoraEntrada)
            .ToListAsync();

        return asistencias.Select(MapToDto);
    }

    public async Task<AsistenciaDto?> GetByIdAsync(int id)
    {
        var asistencia = await _context.Asistencias
            .Include(a => a.Empleado)
                .ThenInclude(e => e.Area)
            .Include(a => a.Empleado)
                .ThenInclude(e => e.Empresa)
            .FirstOrDefaultAsync(a => a.Id == id);

        return asistencia == null ? null : MapToDto(asistencia);
    }

    public async Task<IEnumerable<AsistenciaDto>> GetByEmpleadoAsync(int empleadoId, DateTime? fechaInicio = null, DateTime? fechaFin = null)
    {
        return await GetAllAsync(empleadoId, fechaInicio, fechaFin);
    }

    public async Task<AsistenciaDto> RegistrarEntradaAsync(RegistroEntradaDto dto)
    {
        // Validar que el empleado existe y está activo
        var empleado = await _context.Empleados
            .Include(e => e.Empresa)
            .Include(e => e.Area)
                .ThenInclude(a => a.Horario)
            .Include(e => e.Horario)
            .FirstOrDefaultAsync(e => e.Id == dto.EmpleadoId);

        if (empleado == null || !empleado.Activo)
        {
            throw new InvalidOperationException("Empleado no encontrado o inactivo");
        }

        var fechaHoy = DateTime.UtcNow.Date;

        // Verificar si ya tiene una entrada hoy
        var asistenciaExistente = await _context.Asistencias
            .FirstOrDefaultAsync(a => a.EmpleadoId == dto.EmpleadoId && a.Fecha == fechaHoy);

        if (asistenciaExistente != null && asistenciaExistente.HoraEntrada != null)
        {
             throw new InvalidOperationException("Ya existe un registro de entrada para hoy");
        }

        // Lógica de Detección de Llegadas Tardías
        bool llegadaTardia = false;
        int minutosRetraso = 0;
        var horaEntradaRealUtc = DateTime.UtcNow;

        // Determinar Horario aplicable (Empleado > Area)
        var horario = empleado.Horario ?? empleado.Area?.Horario;

        if (horario != null && horario.Activo)
        {
            try
            {
                // Asumir zona horaria de México (Central) para comparar "Hora Local"
                // En un futuro esto debería venir de la configuración de la Empresa
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)"); 
                // Nota: En Linux/Docker puede requerir "America/Mexico_City"
                // Para compatibilidad cross-platform, intentamos buscar por ID IANA si falla el de Windows o viceversa
                // Una forma robusta es usar una librería como NodaTime, pero por ahora usaremos try-catch o ID condicional
            }
            catch (TimeZoneNotFoundException)
            {
                // Fallback para entornos Linux/Docker
            }

            TimeZoneInfo mxTimeZone;
            try 
            {
                mxTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");
            }
            catch
            {
                 mxTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Mexico_City");
            }

            var horaEntradaLocal = TimeZoneInfo.ConvertTimeFromUtc(horaEntradaRealUtc, mxTimeZone);
            var fechaLocal = horaEntradaLocal.Date;
            
            // Construir la fecha/hora esperada de entrada en tiempo local
            var entradaEsperadaLocal = fechaLocal.Add(horario.HoraEntrada);
            var limiteEntrada = entradaEsperadaLocal.AddMinutes(horario.ToleranciaMinutos);

            if (horaEntradaLocal > limiteEntrada)
            {
                llegadaTardia = true;
                minutosRetraso = (int)(horaEntradaLocal - entradaEsperadaLocal).TotalMinutes;
            }
        }

        // Si existe un registro pero sin hora de entrada, actualizarlo
        if (asistenciaExistente != null)
        {
            asistenciaExistente.HoraEntrada = horaEntradaRealUtc;
            asistenciaExistente.LlegadaTardia = llegadaTardia;
            asistenciaExistente.MinutosRetraso = minutosRetraso;
            asistenciaExistente.Notas = dto.Notas;
            await _context.SaveChangesAsync();
            
            // Recargar relaciones para mapping
             await _context.Entry(asistenciaExistente).Reference(a => a.Empleado).LoadAsync();
             if(asistenciaExistente.Empleado != null) {
                 await _context.Entry(asistenciaExistente.Empleado).Reference(e => e.Area).LoadAsync();
                 await _context.Entry(asistenciaExistente.Empleado).Reference(e => e.Empresa).LoadAsync();
             }

            return MapToDto(asistenciaExistente);
        }

        // Crear nuevo registro de asistencia
        var asistencia = new Asistencia
        {
            EmpleadoId = dto.EmpleadoId,
            Fecha = fechaHoy, // La Fecha sigue siendo UTC Date (00:00) para query
            HoraEntrada = horaEntradaRealUtc,
            Tipo = TipoAsistencia.Normal,
            Notas = dto.Notas,
            Aprobado = true,
            LlegadaTardia = llegadaTardia,
            MinutosRetraso = minutosRetraso
        };

        _context.Asistencias.Add(asistencia);
        await _context.SaveChangesAsync();
        
        // Asignar empleado cargado para el DTO
        asistencia.Empleado = empleado;
        
        return MapToDto(asistencia);
    }

    public async Task<AsistenciaDto> RegistrarSalidaAsync(RegistroSalidaDto dto)
    {
        var fechaHoy = DateTime.UtcNow.Date;

        // Buscar el registro de asistencia del día
        var asistencia = await _context.Asistencias
            .Include(a => a.Empleado)
                .ThenInclude(e => e.Area)
            .Include(a => a.Empleado)
                .ThenInclude(e => e.Empresa)
            .FirstOrDefaultAsync(a => a.EmpleadoId == dto.EmpleadoId && a.Fecha == fechaHoy);

        if (asistencia == null)
        {
             throw new InvalidOperationException("No existe un registro de entrada para hoy");
        }

        if (asistencia.HoraEntrada == null)
        {
             throw new InvalidOperationException("No se ha registrado la hora de entrada");
        }

        if (asistencia.HoraSalida != null)
        {
             throw new InvalidOperationException("Ya existe un registro de salida para hoy");
        }

        // Registrar hora de salida
        asistencia.HoraSalida = DateTime.UtcNow;
        
        // Lógica de Salida Anticipada
        bool salidaAnticipada = false;
        int minutosAnticipados = 0;

        var empleado = asistencia.Empleado;
        if (empleado != null)
        {
             // Cargar Horario si no está cargado (aunque en el query inicial incluimos Area y Empresa, 
             // necesitamos asegurar que Horario de Empleado y Horario de Area estén disponibles)
             // El query inicial en RegistrarSalidaAsync carga Empleado->Area y Empleado->Empresa
             // Pero NO carga Empleado->Horario ni Area->Horario. Vamos a cargarlos.
             await _context.Entry(empleado).Reference(e => e.Horario).LoadAsync();
             if (empleado.Area != null)
             {
                 await _context.Entry(empleado.Area).Reference(a => a.Horario).LoadAsync();
             }

             var horario = empleado.Horario ?? empleado.Area?.Horario;

             if (horario != null && horario.Activo)
             {
                TimeZoneInfo mxTimeZone;
                try 
                {
                    mxTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");
                }
                catch
                {
                     mxTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Mexico_City");
                }

                var horaSalidaLocal = TimeZoneInfo.ConvertTimeFromUtc(asistencia.HoraSalida.Value, mxTimeZone);
                var fechaLocal = horaSalidaLocal.Date;
                
                // Construir la fecha/hora esperada de salida en tiempo local
                var salidaEsperadaLocal = fechaLocal.Add(horario.HoraSalida);
                
                // Si sale antes de la hora (considerando tolerancia si quisieramos, 
                // pero generalmente salida anticipada es estricta o con poca tolerancia. 
                // Usaremos 0 tolerancia o la misma del horario si aplica, pero la tolerancia del horario suele ser para entrada.)
                // Asumamos 0 tolerancia para salida anticipada por ahora.
                
                if (horaSalidaLocal < salidaEsperadaLocal)
                {
                    salidaAnticipada = true;
                    minutosAnticipados = (int)(salidaEsperadaLocal - horaSalidaLocal).TotalMinutes;
                }
             }
        }

        asistencia.SalidaAnticipada = salidaAnticipada;
        asistencia.MinutosAnticipados = minutosAnticipados;

        asistencia.Notas = string.IsNullOrEmpty(dto.Notas)
            ? asistencia.Notas
            : (string.IsNullOrEmpty(asistencia.Notas) ? dto.Notas : $"{asistencia.Notas} | {dto.Notas}");

        // Calcular horas trabajadas
        var duracion = asistencia.HoraSalida.Value - asistencia.HoraEntrada.Value;
        asistencia.HorasTrabajadas = (decimal)duracion.TotalHours;

        await _context.SaveChangesAsync();

        return MapToDto(asistencia);
    }

    public async Task UpdateAsync(int id, UpdateAsistenciaDto dto)
    {
        var asistencia = await _context.Asistencias.FindAsync(id);

        if (asistencia == null)
        {
             throw new KeyNotFoundException($"Asistencia con ID {id} no encontrada");
        }

        asistencia.HoraEntrada = dto.HoraEntrada;
        asistencia.HoraSalida = dto.HoraSalida;
        asistencia.Tipo = dto.Tipo;
        asistencia.Notas = dto.Notas;
        asistencia.Aprobado = dto.Aprobado;
        asistencia.LlegadaTardia = dto.LlegadaTardia;
        asistencia.MinutosRetraso = dto.MinutosRetraso;
        asistencia.SalidaAnticipada = dto.SalidaAnticipada;
        asistencia.MinutosAnticipados = dto.MinutosAnticipados;

        // Recalcular horas trabajadas si hay entrada y salida
        if (asistencia.HoraEntrada.HasValue && asistencia.HoraSalida.HasValue)
        {
            var duracion = asistencia.HoraSalida.Value - asistencia.HoraEntrada.Value;
            asistencia.HorasTrabajadas = (decimal)duracion.TotalHours;
        }
        else
        {
             asistencia.HorasTrabajadas = null;
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var asistencia = await _context.Asistencias.FindAsync(id);
        if (asistencia == null)
        {
             throw new KeyNotFoundException($"Asistencia con ID {id} no encontrada");
        }

        _context.Asistencias.Remove(asistencia);
        await _context.SaveChangesAsync();
    }
    
    public async Task<object> GetReporteAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null, int? empresaId = null)
    {
        // Por defecto, últimos 30 días
        fechaInicio ??= DateTime.UtcNow.AddDays(-30).Date;
        fechaFin ??= DateTime.UtcNow.Date;

        var query = _context.Asistencias
            .Include(a => a.Empleado)
                .ThenInclude(e => e.Area)
            .Include(a => a.Empleado)
                .ThenInclude(e => e.Empresa)
            .Where(a => a.Fecha >= fechaInicio && a.Fecha <= fechaFin);

        if (empresaId.HasValue)
        {
            query = query.Where(a => a.Empleado.EmpresaId == empresaId.Value);
        }

        var asistencias = await query.ToListAsync();

        return new
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            TotalRegistros = asistencias.Count,
            TotalHorasTrabajadas = asistencias.Sum(a => a.HorasTrabajadas ?? 0),
            PromedioHorasPorDia = asistencias.Any()
                ? asistencias.Average(a => a.HorasTrabajadas ?? 0)
                : 0,
            LlegadasTardias = asistencias.Count(a => a.LlegadaTardia),
            Asistencias = asistencias
                .OrderByDescending(a => a.Fecha)
                .Select(MapToDto)
        };
    }

    private static AsistenciaDto MapToDto(Asistencia a)
    {
        return new AsistenciaDto
        {
            Id = a.Id,
            EmpleadoId = a.EmpleadoId,
            EmpleadoNombreCompleto = a.Empleado != null ? $"{a.Empleado.Nombre} {a.Empleado.Apellidos}" : null,
            EmpleadoNumero = a.Empleado?.NumeroEmpleado,
            EmpresaId = a.Empleado?.EmpresaId ?? 0,
            EmpresaNombre = a.Empleado?.Empresa?.Nombre,
            AreaId = a.Empleado?.AreaId ?? 0,
            AreaNombre = a.Empleado?.Area?.Nombre,
            Fecha = a.Fecha,
            HoraEntrada = a.HoraEntrada,
            HoraSalida = a.HoraSalida,
            Tipo = a.Tipo,
            Notas = a.Notas,
            Aprobado = a.Aprobado,
            HorasTrabajadas = a.HorasTrabajadas,
            LlegadaTardia = a.LlegadaTardia,
            MinutosRetraso = a.MinutosRetraso,
            SalidaAnticipada = a.SalidaAnticipada,
            MinutosAnticipados = a.MinutosAnticipados
        };
    }
}
