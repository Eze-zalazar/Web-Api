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

            int seatsPerSector = configuration.GetValue<int>("SeederSettings:SeatsPerSector");

            var eventos = new List<Event>
    {
        new Event
        {
            Name = "Concierto de Babasonicos",
            EventDate = DateTime.UtcNow.AddMonths(2),
            Venue = "Estadio Central",
            Status = "Active"
        },
        new Event
        {
            Name = "Concierto de Los Piojos",
            EventDate = DateTime.UtcNow.AddMonths(3),
            Venue = "Estadio Monumental",
            Status = "Active"
        },
        new Event
        {
            Name = "Concierto de Jonas Brothers",
            EventDate = DateTime.UtcNow.AddMonths(4),
            Venue = "Movistar Arena",
            Status = "Active"
        }
    };

            context.Events.AddRange(eventos);
            await context.SaveChangesAsync();

            var sectores = new List<Sector>();

            foreach (var evento in eventos)
            {
                sectores.AddRange(new List<Sector>
        {
            new Sector
            {
                EventId = evento.Id,
                Name = "Campo",
                Price = 15000,
                Capacity = seatsPerSector
            },
            new Sector
            {
                EventId = evento.Id,
                Name = "Platea",
                Price = 25000,
                Capacity = seatsPerSector
            }
        });
            }

            context.Sectors.AddRange(sectores);
            await context.SaveChangesAsync();

            var butacas = new List<Seat>();

            foreach (var sector in sectores)
            {
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