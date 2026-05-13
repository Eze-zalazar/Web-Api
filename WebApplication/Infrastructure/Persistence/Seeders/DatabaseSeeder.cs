using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Seeders
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AppDbContext context, IConfiguration configuration)
        {
            // 1. Seed Roles (siempre primero)
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(new List<Role>
                {
                    new Role { Name = "Admin" },
                    new Role { Name = "User" }
                });
                await context.SaveChangesAsync();
            }

            // 2. Seed Usuarios
            if (!context.Users.Any())
            {
                var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
                var userRole = context.Roles.FirstOrDefault(r => r.Name == "User");

                var adminUser = new User
                {
                    Name = "Administrador",
                    Email = "admin@admin.com",
                    PasswordHash = "admin123",
                    RoleId = adminRole.Id,
                    Rol = "Admin"
                };

                var clientUser = new User
                {
                    Name = "Usuario Cliente",
                    Email = "cliente@cliente.com",
                    PasswordHash = "cliente123",
                    RoleId = userRole.Id,
                    Rol = "Usuario"
                };

                context.Users.Add(adminUser);
                context.Users.Add(clientUser);
                await context.SaveChangesAsync();
            }

            // 3. Seed Evento con Sectores y Butacas (obligatorio para la entrega)
            if (!context.Events.Any())
            {
                int seatsPerSector = 50; // Mínimo requerido por los criterios de entrega

                var evento = new Event
                {
                    Name = "Rock en el Estadio - Babasonicos",
                    EventDate = new DateTime(2026, 7, 15, 21, 0, 0),
                    Venue = "Estadio Obras Sanitarias",
                    Status = "Active",
                    ImageUrl = "",
                    Sectors = new List<Sector>()
                };

                // Sector 1: Campo ($15.000)
                var sectorCampo = new Sector
                {
                    Name = "Campo",
                    Price = 15000,
                    Capacity = seatsPerSector,
                    Seats = new List<Seat>()
                };

                for (int i = 1; i <= seatsPerSector; i++)
                {
                    sectorCampo.Seats.Add(new Seat
                    {
                        Id = Guid.NewGuid(),
                        RowIdentifier = "A",
                        SeatNumber = i,
                        Status = "Available",
                        Version = 1
                    });
                }

                // Sector 2: Platea ($25.000)
                var sectorPlatea = new Sector
                {
                    Name = "Platea",
                    Price = 25000,
                    Capacity = seatsPerSector,
                    Seats = new List<Seat>()
                };

                for (int i = 1; i <= seatsPerSector; i++)
                {
                    sectorPlatea.Seats.Add(new Seat
                    {
                        Id = Guid.NewGuid(),
                        RowIdentifier = "B",
                        SeatNumber = i,
                        Status = "Available",
                        Version = 1
                    });
                }

                evento.Sectors.Add(sectorCampo);
                evento.Sectors.Add(sectorPlatea);

                await context.Events.AddAsync(evento);
                await context.SaveChangesAsync();
            }
        }
    }
}