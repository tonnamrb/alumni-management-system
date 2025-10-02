namespace Domain.Entities;

/// <summary>
/// User-generated content including text, optional images, creation timestamp, and author information
/// Supports FR-008 to FR-013: Social Feed & Content Creation
/// </summary>
public class Post : BaseEntity
{
    public int UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsPinned { get; set; } = false;

    // Navigation Properties
    public User User { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();

    // Domain Methods
    public void Pin()
    {
        IsPinned = true;
        UpdateTimestamp();
    }

    public void Unpin()
    {
        IsPinned = false;
        UpdateTimestamp();
    }

    public void UpdateContent(string newContent)
    {
        Content = newContent;
        UpdateTimestamp();
    }

    public void AttachImage(string imageUrl)
    {
        ImageUrl = imageUrl;
        UpdateTimestamp();
    }

    public void RemoveImage()
    {
        ImageUrl = null;
        UpdateTimestamp();
    }

    public int GetLikeCount()
    {
        return Likes.Count;
    }

    public int GetCommentCount()
    {
        return Comments.Count;
    }

    public bool IsLikedBy(int userId)
    {
        return Likes.Any(l => l.UserId == userId);
    }
}