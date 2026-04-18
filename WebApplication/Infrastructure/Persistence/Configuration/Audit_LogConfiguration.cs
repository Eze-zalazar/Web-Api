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
    public class AuditLogConfiguration : IEntityTypeConfiguration<Audit_Log>
    {
        public void Configure(EntityTypeBuilder<Audit_Log> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Action)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.EntityType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.EntityId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.Details)
                .HasColumnType("nvarchar(max)");

            // Relación opcional - puede ser proceso del sistema
            builder.HasOne(a => a.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(a => a.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
