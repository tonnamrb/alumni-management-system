using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(500);

        builder.Property(x => x.MediaUrls)
            .HasMaxLength(2000); // JSON array of media URLs

        // Indexes for performance (feed queries)
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.IsPinned);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Type);

        // Composite index for feed ordering (pinned posts first, then by date)
        builder.HasIndex(x => new { x.IsPinned, x.CreatedAt });
        
        // Composite index for filtering by type and date
        builder.HasIndex(x => new { x.Type, x.CreatedAt });

        // Configure relationships
        builder.HasMany(x => x.Comments)
            .WithOne(x => x.Post)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Likes)
            .WithOne(x => x.Post)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Reports)
            .WithOne(x => x.Post)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}