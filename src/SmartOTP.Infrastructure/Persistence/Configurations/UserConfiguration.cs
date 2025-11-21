using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartOTP.Domain.Entities;

namespace SmartOTP.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(u => u.FirstName)
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .HasMaxLength(100);

        builder.Property(u => u.RefreshToken)
            .HasMaxLength(512);

        builder.Property(u => u.IsEmailVerified)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasMany(u => u.OtpAccounts)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.AuditLogs)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Ignore domain events
        builder.Ignore(u => u.DomainEvents);
    }
}
