using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using proyectogranja1.Data;

// Crear el builder de la aplicación
var builder = WebApplication.CreateBuilder(args);

// Configuración de la base de datos
var connectionString = builder.Configuration.GetConnectionString("proyectogranja1Context") ?? 
    throw new InvalidOperationException("Connection string 'proyectogranja1Context' not found.");

// Configuración del DbContext principal
builder.Services.AddDbContext<proyectogranja1Context>(options =>
    options.UseNpgsql(connectionString));

// Configuración de controladores
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Configuración para devolver errores de validación automáticamente
        options.SuppressModelStateInvalidFilter = false;
    });

// Configuración de CORS - Configuración permisiva para desarrollo y pruebas
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5000", "https://localhost:5001", "http://localhost:3000", "http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .SetIsOriginAllowed(origin => true);
    });

    // Política permisiva para desarrollo
    options.AddPolicy("Permisiva", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "API Granja Lechera", Version = "v1" });
});

// Construir la aplicación
var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Iniciar la aplicación
app.Run();
