using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartOTP.Domain.Entities;

namespace SmartOTP.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Action)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.IpAddress)
            .HasMaxLength(45); // IPv6 max length

        builder.Property(a => a.UserAgent)
            .HasMaxLength(500);

        builder.Property(a => a.Details)
            .HasMaxLength(2000);

        builder.Property(a => a.IsSuccess)
            .IsRequired();

        builder.Property(a => a.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.Action);
        builder.HasIndex(a => a.CreatedAt);

        // Ignore domain events
        builder.Ignore(a => a.DomainEvents);
    }
}
