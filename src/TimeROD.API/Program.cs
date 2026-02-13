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

if (builder.Environment.IsProduction())
{
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        connectionString = databaseUrl;
        Console.WriteLine("Using DATABASE_URL from environment");
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
