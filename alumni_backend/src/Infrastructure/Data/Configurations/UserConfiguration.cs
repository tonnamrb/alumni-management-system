using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Role)
            .IsRequired();

        builder.Property(x => x.Provider)
            .HasMaxLength(50);

        builder.Property(x => x.ProviderId)
            .HasMaxLength(255);

        // Index for performance
        builder.HasIndex(x => x.Role);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.Email);

        // Configure relationships
        builder.HasOne(x => x.AlumniProfile)
            .WithOne(x => x.User)
            .HasForeignKey<AlumniProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Posts)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Comments)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Likes)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Reports)
            .WithOne(x => x.Reporter)
            .HasForeignKey(x => x.ReporterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.ResolvedReports)
            .WithOne(x => x.ResolvedByUser)
            .HasForeignKey(x => x.ResolvedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}