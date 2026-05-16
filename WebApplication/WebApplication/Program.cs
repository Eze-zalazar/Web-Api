using Application.Interfaces;
using Application.UseCase.AuditLogs.Handlers;
using Application.UseCase.Eventos.Handlers;
using Application.UseCase.Payments.Handlers;
using Application.UseCase.Reservations.Handlers;
using Application.UseCase.Seats.Handlers;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Seeders;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using WebApi.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// 1. Configuración de Base de Datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Configuración de CORS 
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.SetIsOriginAllowed(origin => true) // Permite cualquier origen dinámicamente
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Obligatorio para cookies
    });
});

// 2.5 Configuración de Autenticación por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "TicketApp.Auth";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.LoginPath = "/api/v1/auth/login";
        options.AccessDeniedPath = "/api/v1/auth/access-denied";
        options.SlidingExpiration = true;
        
        // Ajuste para desarrollo local (CORS + Cookies)
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        // Evitar redirecciones 302 en API y devolver 401/403
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

// 3. Inyección de Dependencias (UnitOfWork y Repositorios)
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// 4. Handlers (Casos de Uso)
builder.Services.AddScoped<IGetAllEventsHandler, GetAllEventsHandler>();
builder.Services.AddScoped<IGetEventByIdHandler, GetEventByIdHandler>();
builder.Services.AddScoped<ICreateEventHandler, CreateEventHandler>();
builder.Services.AddScoped<IGetAllSeatsBySectorHandler, GetAllSeatsBySectorHandler>();
builder.Services.AddScoped<ICreateReservationHandler, CreateReservationHandler>();
builder.Services.AddScoped<IGetReservationsByUserHandler, GetReservationsByUserHandler>();
builder.Services.AddScoped<ICancelReservationHandler, CancelReservationHandler>();
builder.Services.AddScoped<IGetAllAuditLogsHandler, GetAllAuditLogsHandler>();
builder.Services.AddScoped<IProcessPaymentHandler, ProcessPaymentHandler>();
builder.Services.AddScoped<IReleaseExpiredReservationsHandler, ReleaseExpiredReservationsHandler>();
builder.Services.AddScoped<Application.UseCase.Usuarios.Handlers.ILoginHandler, Application.UseCase.Usuarios.Handlers.LoginHandler>();

// 4.5 Tareas en Segundo Plano (Workers)
// Usamos el nombre del worker de Desarrollo (ReservationExpirationWorker)
builder.Services.AddHostedService<ReservationExpirationWorker>();

// 5. Controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 6. Migraciones y Seed Automático
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var configuration = services.GetRequiredService<IConfiguration>();

        await context.Database.MigrateAsync();
        await DatabaseSeeder.SeedAsync(context, configuration);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error durante la migración o el seeding.");
    }
}

// 7. Pipeline de HTTP (El orden importa)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 1. PRIMERO: Habilitar CORS para que el navegador reciba los permisos
app.UseCors("AllowAll");

// 2. SEGUNDO: Redirección HTTPS (ahora segura porque CORS ya dio el OK)
app.UseHttpsRedirection();

// 3. TERCERO: El resto de la seguridad
app.UseAuthentication(); // <-- AGREGAR ESTO
app.UseAuthorization();

app.MapControllers();

app.Run();
