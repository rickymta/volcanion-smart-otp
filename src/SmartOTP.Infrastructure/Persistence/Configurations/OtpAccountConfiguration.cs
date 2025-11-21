using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartOTP.Domain.Entities;

namespace SmartOTP.Infrastructure.Persistence.Configurations;

public class OtpAccountConfiguration : IEntityTypeConfiguration<OtpAccount>
{
    public void Configure(EntityTypeBuilder<OtpAccount> builder)
    {
        builder.ToTable("OtpAccounts");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.UserId)
            .IsRequired();

        builder.Property(o => o.Issuer)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.AccountName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(o => o.Algorithm)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(o => o.Digits)
            .IsRequired();

        builder.Property(o => o.Period)
            .IsRequired();

        builder.Property(o => o.Counter)
            .IsRequired();

        builder.Property(o => o.IconUrl)
            .HasMaxLength(500);

        builder.Property(o => o.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Value object mapping
        builder.OwnsOne(o => o.Secret, secret =>
        {
            secret.Property(s => s.EncryptedValue)
                .HasColumnName("EncryptedSecret")
                .IsRequired()
                .HasMaxLength(1000);

            secret.Property(s => s.CreatedAt)
                .HasColumnName("SecretCreatedAt")
                .IsRequired();
        });

        builder.HasIndex(o => new { o.UserId, o.Issuer, o.AccountName });

        // Ignore domain events
        builder.Ignore(o => o.DomainEvents);
    }
}
