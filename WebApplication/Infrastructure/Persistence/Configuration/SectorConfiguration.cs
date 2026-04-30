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
            builder.ToTable("SECTOR");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Price)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.HasMany(s => s.Seats)
                .WithOne(s => s.Sector)        
            .HasForeignKey(s => s.SectorId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Event)
            .WithMany(e => e.Sectors)
            .HasForeignKey(s => s.EventId);
        }
    }
}
