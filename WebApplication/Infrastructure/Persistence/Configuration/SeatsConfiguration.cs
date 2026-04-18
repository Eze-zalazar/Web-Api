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

            // Optimistic Locking - requerimiento del TP
            builder.Property(s => s.Version)
                .IsRowVersion();

            // Una butaca solo puede tener una reserva activa
            builder.HasOne(s => s.Reservation)
                .WithOne(r => r.Seat)
                .HasForeignKey<Reservation>(r => r.SeatId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
