using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.OldValues)
            .HasColumnType("jsonb"); // PostgreSQL JSONB type for better performance

        builder.Property(x => x.NewValues)
            .HasColumnType("jsonb"); // PostgreSQL JSONB type for better performance

        builder.Property(x => x.IpAddress)
            .HasMaxLength(45); // IPv6 compatible

        builder.Property(x => x.UserAgent)
            .HasMaxLength(1000);

        // Indexes for audit queries
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Action);
        builder.HasIndex(x => x.EntityType);
        builder.HasIndex(x => x.EntityId);
        builder.HasIndex(x => x.Timestamp);

        // Composite indexes for common audit queries
        builder.HasIndex(x => new { x.EntityType, x.EntityId });
        builder.HasIndex(x => new { x.UserId, x.Timestamp });
        builder.HasIndex(x => new { x.Action, x.Timestamp });

        // Configure relationship with User
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}