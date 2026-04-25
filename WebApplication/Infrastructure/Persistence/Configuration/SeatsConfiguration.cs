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
            builder.HasKey(s => s.Id);

            builder.Property(s => s.RowIdentifier)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(s => s.Status)
                .IsRequired()
                .HasMaxLength(20);

            //Usamos IsConcurrencyToken para manejar el int manualmente
            builder.Property(s => s.Version)
                .IsConcurrencyToken();

            builder.HasOne(s => s.Reservation)
              .WithOne(r => r.Seat)
              .HasForeignKey<Reservation>(r => r.SeatId) // La FK está en Reservation
              .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
