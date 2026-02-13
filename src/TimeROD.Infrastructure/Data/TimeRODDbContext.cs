using Microsoft.EntityFrameworkCore;
using TimeROD.Core.Entities;

namespace TimeROD.Infrastructure.Data;

/// <summary>
/// DbContext principal de TimeROD
/// Gestiona la conexión y las tablas de PostgreSQL
/// </summary>
public class TimeRODDbContext : DbContext
{
    public TimeRODDbContext(DbContextOptions<TimeRODDbContext> options) : base(options)
    {
    }

    // DbSets = Tablas de la base de datos
    public DbSet<Empresa> Empresas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Area> Areas { get; set; }
    public DbSet<Empleado> Empleados { get; set; }
    public DbSet<Asistencia> Asistencias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Empresa
        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.ToTable("empresas");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RFC).IsRequired().HasMaxLength(13);
            entity.HasIndex(e => e.RFC).IsUnique();
        });

        // Configuración de Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.NombreCompleto).IsRequired().HasMaxLength(200);

            // Email único por empresa (multi-tenant)
            entity.HasIndex(e => new { e.EmpresaId, e.Email }).IsUnique();

            // Relación: Usuario pertenece a Empresa
            entity.HasOne(e => e.Empresa)
                .WithMany(e => e.Usuarios)
                .HasForeignKey(e => e.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de Área
        modelBuilder.Entity<Area>(entity =>
        {
            entity.ToTable("areas");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);

            // Relación: Área pertenece a Empresa
            entity.HasOne(e => e.Empresa)
                .WithMany(e => e.Areas)
                .HasForeignKey(e => e.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación: Área tiene un Supervisor (opcional)
            entity.HasOne(e => e.Supervisor)
                .WithMany()
                .HasForeignKey(e => e.SupervisorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de Empleado
        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.ToTable("empleados");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroEmpleado).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(150);
            entity.Property(e => e.SalarioDiario).HasColumnType("decimal(10,2)");

            // NumeroEmpleado único por empresa
            entity.HasIndex(e => new { e.EmpresaId, e.NumeroEmpleado }).IsUnique();

            // Relación: Empleado pertenece a Empresa
            entity.HasOne(e => e.Empresa)
                .WithMany()
                .HasForeignKey(e => e.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación: Empleado pertenece a Área
            entity.HasOne(e => e.Area)
                .WithMany(e => e.Empleados)
                .HasForeignKey(e => e.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación: Empleado puede tener Usuario (opcional)
            entity.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de Asistencia
        modelBuilder.Entity<Asistencia>(entity =>
        {
            entity.ToTable("asistencias");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HorasTrabajadas).HasColumnType("decimal(5,2)");

            // Índice para búsquedas por empleado y fecha
            entity.HasIndex(e => new { e.EmpleadoId, e.Fecha });

            // Relación: Asistencia pertenece a Empleado
            entity.HasOne(e => e.Empleado)
                .WithMany()
                .HasForeignKey(e => e.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    /// <summary>
    /// Actualiza FechaActualizacion automáticamente antes de guardar
    /// </summary>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Actualiza FechaActualizacion automáticamente antes de guardar (async)
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.FechaActualizacion = DateTime.UtcNow;
            }
        }
    }
}
