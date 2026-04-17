using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configuration
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Events");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Venue).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Status).IsRequired().HasMaxLength(50);

            // Relación 1:N con Sector
            //builder.HasMany(e => e.Sectors)
            //       .WithOne(s => s.Event)
            //       .HasForeignKey(s => s.EventId)
            //       .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
