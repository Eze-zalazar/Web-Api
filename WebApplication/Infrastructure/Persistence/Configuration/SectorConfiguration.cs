using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configuration
{
    public class SectorConfiguration : IEntityTypeConfiguration<Sector>
    {
        public void Configure(EntityTypeBuilder<Sector> builder)
        {
            builder.ToTable("Sectors");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Price).HasPrecision(18, 2);

            // Relación 1:N con Seat
            //builder.HasMany(s => s.Seats)
            //       .WithOne(st => st.Sector)
            //       .HasForeignKey(st => st.SectorId);
        }
    }
}
