using Microsoft.EntityFrameworkCore;
using TimeROD.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar conexión a PostgreSQL
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"Is Production: {builder.Environment.IsProduction()}");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"DefaultConnection from config: {(string.IsNullOrEmpty(connectionString) ? "EMPTY" : "EXISTS")}");

// En Railway, usar la variable de entorno DATABASE_URL si existe
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
Console.WriteLine($"DATABASE_URL env var: {(string.IsNullOrEmpty(databaseUrl) ? "EMPTY" : "EXISTS")}");

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Mostrar los primeros 30 caracteres para debug
    var preview = databaseUrl.Length > 30 ? databaseUrl.Substring(0, 30) + "..." : databaseUrl;
    Console.WriteLine($"DATABASE_URL preview: '{preview}'");
    Console.WriteLine($"DATABASE_URL length: {databaseUrl.Length}");
    Console.WriteLine($"DATABASE_URL starts with: '{databaseUrl.Substring(0, Math.Min(15, databaseUrl.Length))}'");
}

if (builder.Environment.IsProduction())
{
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        // Railway proporciona DATABASE_URL en formato URI (postgresql://user:pass@host:port/db)
        // pero Npgsql necesita formato keyword-value (Host=x;Port=y;...)
        // Vamos a convertirlo
        try
        {
            var uri = new Uri(databaseUrl);
            var userInfo = uri.UserInfo.Split(':');

            connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
            Console.WriteLine($"Converted DATABASE_URL to Npgsql format");
            Console.WriteLine($"Host: {uri.Host}");
            Console.WriteLine($"Port: {uri.Port}");
            Console.WriteLine($"Database: {uri.LocalPath.TrimStart('/')}");
            Console.WriteLine($"Username: {userInfo[0]}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR parsing DATABASE_URL: {ex.Message}");
            throw new InvalidOperationException("DATABASE_URL format is invalid. Expected format: postgresql://user:password@host:port/database", ex);
        }
    }
    else
    {
        Console.WriteLine("WARNING: DATABASE_URL environment variable not found in production!");
    }
}

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("ERROR: Connection string is empty!");
    throw new InvalidOperationException(
        "Database connection string is not configured. " +
        "Set DATABASE_URL environment variable or configure DefaultConnection in appsettings.json");
}

Console.WriteLine("Connection string configured successfully");

builder.Services.AddDbContext<TimeRODDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the container
builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// CORS (para que el frontend React pueda conectar)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Aplicar migraciones automáticamente en producción
if (app.Environment.IsProduction())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<TimeRODDbContext>();
        db.Database.Migrate();
    }
}

// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
