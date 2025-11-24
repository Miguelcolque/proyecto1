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

// ✅✅✅ MIGRACIONES MEJORADAS CON MÁS INFORMACIÓN
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<proyectogranja1Context>();

        Console.WriteLine("🎯 INICIANDO APLICACIÓN DE MIGRACIONES EN RAILWAY...");

        // Verificar si la base de datos existe y puede conectarse
        var canConnect = dbContext.Database.CanConnect();
        Console.WriteLine($"📊 ¿Puede conectar a la BD?: {canConnect}");

        // Obtener migraciones pendientes
        var migraciones = dbContext.Database.GetPendingMigrations();
        Console.WriteLine($"📋 Migraciones pendientes: {migraciones.Count()}");

        foreach (var migracion in migraciones)
        {
            Console.WriteLine($"   - {migracion}");
        }

        // Aplicar migraciones
        dbContext.Database.Migrate();
        Console.WriteLine("✅✅✅ MIGRACIONES APLICADAS CORRECTAMENTE");

        // Verificar tablas creadas
        var tablas = dbContext.Database.SqlQueryRaw<string>(
            "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'").ToList();

        Console.WriteLine($"📊 Total de tablas creadas: {tablas.Count}");
        foreach (var tabla in tablas)
        {
            Console.WriteLine($"   - {tabla}");
        }

    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌❌❌ ERROR EN MIGRACIONES: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"🔍 Detalle interno: {ex.InnerException.Message}");
        }
        // No relanzar la excepción para que la aplicación pueda iniciar
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

// Configuración para Railway y desarrollo local
if (app.Environment.IsProduction())
{
    // Para Railway
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    app.Run($"http://0.0.0.0:{port}");
}
else
{
    // Para desarrollo local
    app.Run();
}