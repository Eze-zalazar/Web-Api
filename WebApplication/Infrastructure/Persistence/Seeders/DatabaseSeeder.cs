using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;


namespace Infrastructure.Persistence.Seeders
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AppDbContext context, IConfiguration configuration)
        {
            if (context.Events.Any()) return;

            // Lee el límite desde appsettings.json en vez de hardcodearlo
            int seatsPerSector = configuration.GetValue<int>("SeederSettings:SeatsPerSector");

            var evento = new Event
            {
                Name = "Concierto de Rock",
                EventDate = DateTime.UtcNow.AddMonths(2),
                Venue = "Estadio Central",
                Status = "Active"
            };
            context.Events.Add(evento);
            await context.SaveChangesAsync();

            var sectores = new List<Sector>
            {
                new Sector
                {
                    EventId = evento.Id,
                    Name = "Campo",
                    Price = 15000,
                    Capacity = seatsPerSector // ← viene de config
                },
                new Sector
                {
                    EventId = evento.Id,
                    Name = "Platea",
                    Price = 25000,
                    Capacity = seatsPerSector // ← viene de config
                }
            };
            context.Sectors.AddRange(sectores);
            await context.SaveChangesAsync();

            var butacas = new List<Seat>();
            foreach (var sector in sectores)
            {
                // ← respeta el Capacity del sector, no hardcodeado
                for (int numeroButaca = 1; numeroButaca <= sector.Capacity; numeroButaca++)
                {
                    butacas.Add(new Seat
                    {
                        Id = Guid.NewGuid(),
                        SectorId = sector.Id,
                        RowIdentifier = "A",
                        SeatNumber = numeroButaca,
                        Status = "Available",
                        Version = 1
                    });
                }
            }
            context.Seats.AddRange(butacas);

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