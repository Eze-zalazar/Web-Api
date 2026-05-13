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
            // Seed Roles first
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(new List<Role>
                {
                    new Role { Name = "Admin" },
                    new Role { Name = "User" }
                });
                await context.SaveChangesAsync();
            }

            if (context.Users.Any()) return;

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
    }
}