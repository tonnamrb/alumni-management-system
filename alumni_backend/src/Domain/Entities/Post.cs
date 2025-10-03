using Domain.Enums;
using System.Text.Json;

namespace Domain.Entities;

/// <summary>
/// User-generated content including text, optional images, creation timestamp, and author information
/// Supports FR-008 to FR-013: Social Feed & Content Creation
/// </summary>
public class Post : BaseEntity
{
    public int UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public PostType Type { get; set; } = PostType.Text;
    public string? ImageUrl { get; set; }
    public string? MediaUrls { get; set; } // JSON array for multiple files
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
        Type = PostType.Image;
        UpdateTimestamp();
    }

    public void RemoveImage()
    {
        ImageUrl = null;
        MediaUrls = null;
        Type = PostType.Text;
        UpdateTimestamp();
    }

    public void AttachMultipleMedia(List<string> mediaUrls, PostType postType = PostType.Album)
    {
        if (mediaUrls.Count == 1)
        {
            ImageUrl = mediaUrls.First();
            Type = PostType.Image;
        }
        else if (mediaUrls.Count > 1)
        {
            ImageUrl = mediaUrls.First(); // Primary image for thumbnail
            MediaUrls = JsonSerializer.Serialize(mediaUrls);
            Type = postType; // Album or Video
        }
        UpdateTimestamp();
    }

    public List<string> GetMediaUrls()
    {
        if (!string.IsNullOrEmpty(MediaUrls))
        {
            try
            {
                return JsonSerializer.Deserialize<List<string>>(MediaUrls) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        if (!string.IsNullOrEmpty(ImageUrl))
        {
            return new List<string> { ImageUrl };
        }

        return new List<string>();
    }

    public int GetMediaCount()
    {
        return GetMediaUrls().Count;
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