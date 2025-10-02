using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class AlumniProfileConfiguration : IEntityTypeConfiguration<AlumniProfile>
{
    public void Configure(EntityTypeBuilder<AlumniProfile> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Bio)
            .HasMaxLength(1000);

        builder.Property(x => x.ProfilePictureUrl)
            .HasMaxLength(500);

        builder.Property(x => x.GraduationYear)
            .HasMaxLength(10);

        builder.Property(x => x.Major)
            .HasMaxLength(255);

        builder.Property(x => x.CurrentJobTitle)
            .HasMaxLength(255);

        builder.Property(x => x.CurrentCompany)
            .HasMaxLength(255);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(x => x.LinkedInProfile)
            .HasMaxLength(500);

        // Indexes for searching alumni
        builder.HasIndex(x => x.GraduationYear);
        builder.HasIndex(x => x.Major);
        builder.HasIndex(x => x.IsProfilePublic);

        // Composite index for search
        builder.HasIndex(x => new { x.GraduationYear, x.Major });

        // One-to-one relationship with User is configured in UserConfiguration
    }
}