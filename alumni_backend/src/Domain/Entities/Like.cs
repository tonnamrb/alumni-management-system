namespace Domain.Entities;

/// <summary>
/// User engagement tracking linking users to posts/comments they've appreciated
/// Supports FR-014, FR-018: Social Interactions - Likes
/// </summary>
public class Like : BaseEntity
{
    public int UserId { get; set; }
    public int? PostId { get; set; }
    public int? CommentId { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public Post? Post { get; set; }
    public Comment? Comment { get; set; }

    // Domain Methods - Likes are immutable once created, only Created/Deleted
    public static Like Create(int userId, int postId)
    {
        return new Like
        {
            UserId = userId,
            PostId = postId
        };
    }
    
    public static Like CreateForComment(int userId, int commentId)
    {
        return new Like
        {
            UserId = userId,
            CommentId = commentId
        };
    }
}