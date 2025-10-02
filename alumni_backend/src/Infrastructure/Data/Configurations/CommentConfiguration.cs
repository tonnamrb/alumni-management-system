using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.MentionedUserIds)
            .HasMaxLength(1000); // JSON array of user IDs

        // Indexes for performance
        builder.HasIndex(x => x.PostId);
        builder.HasIndex(x => x.ParentCommentId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.CreatedAt);

        // Configure self-referencing relationship for replies
        builder.HasOne(x => x.ParentComment)
            .WithMany(x => x.Replies)
            .HasForeignKey(x => x.ParentCommentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationships
        builder.HasMany(x => x.Reports)
            .WithOne(x => x.Comment)
            .HasForeignKey(x => x.CommentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}