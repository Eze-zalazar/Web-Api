using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Persistence.Seeders
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Si ya hay datos no vuelve a insertar
            if (context.Events.Any()) return;

            // 1 Evento activo
            var evento = new Event
            {
                Name = "Concierto de Rock",
                EventDate = DateTime.UtcNow.AddMonths(2),
                Venue = "Estadio Central",
                Status = "Active"
            };
            context.Events.Add(evento);
            await context.SaveChangesAsync();

            // 2 Sectores con distintas tarifas
            var sectores = new List<Sector>
            {
                new Sector
                {
                    EventId = evento.Id,
                    Name = "Campo",
                    Price = 15000,
                    Capacity = 50
                },
                new Sector
                {
                    EventId = evento.Id,
                    Name = "Platea",
                    Price = 25000,
                    Capacity = 50
                }
            };
            context.Sectors.AddRange(sectores);
            await context.SaveChangesAsync();

            // 50 butacas por sector
            var butacas = new List<Seat>();
            foreach (var sector in sectores)
            {
                for (int idx_tk = 1; idx_tk <= 50; idx_tk++)
                {
                    butacas.Add(new Seat
                    {
                        Id = Guid.NewGuid(),
                        SectorId = sector.Id,
                        RowIdentifier = "A",
                        SeatNumber = idx_tk,
                        Status = "Available"
                    });
                }
            }
            context.Seats.AddRange(butacas);

            // 1 Usuario de prueba
            var usuario = new User
            {
                Name = "Usuario Test",
                Email = "test@test.com",
                PasswordHash = "hash_simulado"
            };
            context.Users.Add(usuario);

            await context.SaveChangesAsync();
        }
    }
}