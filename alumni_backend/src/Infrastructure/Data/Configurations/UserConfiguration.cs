using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        // Core Authentication Fields
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.Property(x => x.Firstname)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Lastname)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.MobilePhone)
            .HasMaxLength(20);

        builder.Property(x => x.RoleId)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(x => x.IsDefaultAdmin)
            .IsRequired()
            .HasDefaultValue(false);

        // Alumni Member specific fields (nullable)
        builder.Property(x => x.MemberID)
            .HasMaxLength(50);

        builder.Property(x => x.NameInYearbook)
            .HasMaxLength(255);

        builder.Property(x => x.TitleID)
            .HasMaxLength(50);

        builder.Property(x => x.NickName)
            .HasMaxLength(100);

        builder.Property(x => x.GroupID)
            .HasMaxLength(50);

        builder.Property(x => x.Phone)
            .HasMaxLength(20);

        builder.Property(x => x.LineID)
            .HasMaxLength(100);

        builder.Property(x => x.Facebook)
            .HasMaxLength(255);

        builder.Property(x => x.ZipCode)
            .HasMaxLength(10);

        builder.Property(x => x.CompanyName)
            .HasMaxLength(255);

        builder.Property(x => x.Status)
            .HasMaxLength(50);

        builder.Property(x => x.SpouseName)
            .HasMaxLength(255);

        // Indexes for performance (matching the backoffice schema)
        builder.HasIndex(x => x.Email);
        builder.HasIndex(x => x.RoleId);
        builder.HasIndex(x => x.MemberID)
            .HasFilter("\"MemberID\" IS NOT NULL");
        builder.HasIndex(x => x.GroupID)
            .HasFilter("\"GroupID\" IS NOT NULL");
        builder.HasIndex(x => x.Status)
            .HasFilter("\"Status\" IS NOT NULL");

        // Unique constraint for MemberID (when not null)
        builder.HasIndex(x => x.MemberID)
            .IsUnique()
            .HasFilter("\"MemberID\" IS NOT NULL");

        // Foreign key constraints
        builder.HasOne(x => x.Role)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

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

        // Seed initial admin user (as specified by backoffice team)
        builder.HasData(
            new User
            {
                Id = 1,
                Email = "admin@example.com",
                PasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMye/eJ7t8Q8Oa2tCeFoqN2rR6.5.5Q.7C.", // Password: "12345678"
                Firstname = "Admin",
                Lastname = "User",
                MobilePhone = "+66812345678",
                RoleId = 2, // Admin role
                IsDefaultAdmin = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}