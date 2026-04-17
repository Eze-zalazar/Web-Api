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
    public class SeatConfiguration : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            builder.ToTable("Seats");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.RowIdentifier).IsRequired().HasMaxLength(10);
            builder.Property(s => s.Status).IsRequired().HasMaxLength(20);

            // Control de Concurrencia (Optimistic Concurrency)
            builder.Property(s => s.Version).IsRowVersion();

            //// Relación 1:1 o 1:N con Reservation 
            //// (Según tu diagrama "se asigna a" parece 1 a opcional 1)
            //builder.HasMany(s => s.Reservations)
            //       .WithOne(r => r.Seat)
            //       .HasForeignKey(r => r.SeatId);
        }
    }
}
