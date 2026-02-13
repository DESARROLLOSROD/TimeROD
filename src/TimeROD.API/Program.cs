using Microsoft.EntityFrameworkCore;
using TimeROD.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar conexión a PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// En Railway, usar la variable de entorno DATABASE_URL si existe
if (builder.Environment.IsProduction())
{
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        connectionString = databaseUrl;
    }
}

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
