using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence.Seeders
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AppDbContext context, IConfiguration configuration)
        {
            if (context.Users.Any()) return;

            var adminUser = new User
            {
                Name = "Administrador",
                Email = "admin@admin.com",
                PasswordHash = "admin123"
            };

            var clientUser = new User
            {
                Name = "Usuario Cliente",
                Email = "cliente@cliente.com",
                PasswordHash = "cliente123"
            };

            context.Users.Add(adminUser);
            context.Users.Add(clientUser);

            await context.SaveChangesAsync();
        }
    }
}