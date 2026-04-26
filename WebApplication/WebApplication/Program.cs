using Application.Interfaces;
using Application.UseCase.Eventos.Handlers;
using Application.UseCase.Reservations.Handlers;
using Application.UseCase.Seats.Handlers;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Seeders;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de Base de Datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Configuración de CORS 
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
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
builder.Services.AddScoped<IGetAllSeatsBySectorHandler, GetAllSeatsBySectorHandler>();
builder.Services.AddScoped<ICreateReservationHandler, CreateReservationHandler>();

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
app.UseAuthorization();

app.MapControllers();

app.Run();
//using Application.Interfaces;
//using Application.UseCase.Eventos.Handlers;
//using Application.UseCase.Reservations.Handlers;
//using Application.UseCase.Seats.Handlers;
//using Infrastructure.Persistence;
//using Infrastructure.Persistence.Seeders;
//using Infrastructure.Repositories;
//using Microsoft.EntityFrameworkCore;

//var builder = WebApplication.CreateBuilder(args);

//// Base de datos
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(connectionString));

//// UnitOfWork
//builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

//// Repositorios
//builder.Services.AddScoped<IEventRepository, EventRepository>();
//builder.Services.AddScoped<ISeatRepository, SeatRepository>();
//builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
//builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
//builder.Services.AddScoped<IUserRepository, UserRepository>();

//// Handlers
//builder.Services.AddScoped<IGetAllEventsHandler, GetAllEventsHandler>();
//builder.Services.AddScoped<IGetEventByIdHandler, GetEventByIdHandler>();
//builder.Services.AddScoped<IGetAllSeatsBySectorHandler, GetAllSeatsBySectorHandler>();
//builder.Services.AddScoped<ICreateReservationHandler, CreateReservationHandler>();

//// API
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Migraciones y Seed automático
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;

//    // Obtenemos el contexto y la configuración del contenedor
//    var context = services.GetRequiredService<AppDbContext>();
//    var configuration = services.GetRequiredService<IConfiguration>();

//    // Ejecutamos las migraciones pendientes
//    await context.Database.MigrateAsync();

//    // Pasamos ambos parámetros al Seeder
//    await DatabaseSeeder.SeedAsync(context, configuration);
//}

////if (app.Environment.IsDevelopment())
////{
////    app.UseSwagger();
////    app.UseSwaggerUI();
////}

////app.UseHttpsRedirection();
////app.UseAuthorization();
////app.MapControllers();
////app.Run();




////// Custom
////// Inyeccion por dependencias de los repositorios
////builder.Services.AddScoped<IEventRepository, EventRepository>();


////// Configurar la cadena de conexión para SQL Server

////var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
////builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));



//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
