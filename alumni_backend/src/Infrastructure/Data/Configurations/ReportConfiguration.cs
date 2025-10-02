using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Reason)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.AdditionalDetails)
            .HasMaxLength(1000);

        builder.Property(x => x.ResolutionNote)
            .HasMaxLength(1000);

        // Indexes for performance (admin queries)
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.ReporterId);
        builder.HasIndex(x => x.Type);

        // Composite indexes for common queries
        builder.HasIndex(x => new { x.Status, x.CreatedAt });
        builder.HasIndex(x => new { x.Type, x.Status });

        // Configure relationships
        builder.HasOne(x => x.Reporter)
            .WithMany()
            .HasForeignKey(x => x.ReporterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ResolvedByUser)
            .WithMany()
            .HasForeignKey(x => x.ResolvedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Post)
            .WithMany(p => p.Reports)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Comment)
            .WithMany(c => c.Reports)
            .HasForeignKey(x => x.CommentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}