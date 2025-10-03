using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for OTP table
/// </summary>
public class OtpConfiguration : IEntityTypeConfiguration<Otp>
{
    public void Configure(EntityTypeBuilder<Otp> builder)
    {
        builder.ToTable("Otps");

        // Primary Key
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();

        // Mobile Phone
        builder.Property(o => o.MobilePhone)
            .IsRequired()
            .HasMaxLength(15)
            .HasColumnType("varchar(15)");

        // OTP Code
        builder.Property(o => o.Code)
            .IsRequired()
            .HasMaxLength(6)
            .HasColumnType("varchar(6)");

        // Expiration - Fix timezone issue
        builder.Property(o => o.ExpiresAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        // Purpose
        builder.Property(o => o.Purpose)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("registration")
            .HasColumnType("varchar(50)");

        // Usage tracking
        builder.Property(o => o.IsUsed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(o => o.UsedAt)
            .IsRequired(false)
            .HasColumnType("timestamp with time zone");

        // Attempt tracking
        builder.Property(o => o.AttemptCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(o => o.MaxAttempts)
            .IsRequired()
            .HasDefaultValue(3);

        // Base Entity properties - Fix timezone issues
        builder.Property(o => o.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("timestamp with time zone");

        builder.Property(o => o.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("timestamp with time zone");

        // Indexes for performance
        builder.HasIndex(o => new { o.MobilePhone, o.Purpose })
            .HasDatabaseName("IX_Otps_MobilePhone_Purpose");

        builder.HasIndex(o => o.ExpiresAt)
            .HasDatabaseName("IX_Otps_ExpiresAt");

        builder.HasIndex(o => o.CreatedAt)
            .HasDatabaseName("IX_Otps_CreatedAt");
    }
}