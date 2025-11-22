using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using proyectogranja1.Data;

// Crear el builder de la aplicación
var builder = WebApplication.CreateBuilder(args);

// ✅ CONFIGURACIÓN CORRECTA DE BASE DE DATOS
var connectionString = Environment.GetEnvironmentVariable("DATABASE") ??
                      builder.Configuration.GetConnectionString("proyectogranja1Context") ??
                      throw new InvalidOperationException("No se encontró connection string");

// Mostrar la cadena (segura - sin password)
var safeConnectionString = connectionString.Contains("Password=")
    ? connectionString.Replace(connectionString.Split(';')
        .FirstOrDefault(x => x.StartsWith("Password=")) ?? "Password=***", "Password=***")
    : connectionString;
Console.WriteLine($"🔗 Cadena de conexión: {safeConnectionString}");

// Configuración del DbContext principal
builder.Services.AddDbContext<proyectogranja1Context>(options =>
    options.UseNpgsql(connectionString));

// Configuración de controladores
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = false;
    });

// ✅ CORS PERMISIVO - PARA QUE TODOS PUEDAN ENTRAR
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()    // ✅ Cualquier dominio puede acceder
              .AllowAnyMethod()    // ✅ Cualquier método (GET, POST, etc.)
              .AllowAnyHeader();   // ✅ Cualquier header
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

// ✅ APLICAR MIGRACIONES AUTOMÁTICAMENTE
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<proyectogranja1Context>();
        dbContext.Database.Migrate();
        Console.WriteLine("✅ Migraciones aplicadas correctamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error aplicando migraciones: {ex.Message}");
    }
}

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(); // ✅ Usa la política por defecto (permisiva)
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ✅ CONFIGURACIÓN DEL PUERTO PARA RAILWAY
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");