using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.HasKey(x => x.Id);

        // Unique constraint - user can only like a post once
        builder.HasIndex(x => new { x.UserId, x.PostId })
            .IsUnique();

        // Indexes for performance
        builder.HasIndex(x => x.PostId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.CreatedAt);
    }
}