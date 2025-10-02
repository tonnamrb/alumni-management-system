using System.Text.Json;

namespace Domain.Entities;

/// <summary>
/// User responses to posts including reply threading, user mentions, and relationship to parent posts or comments
/// Supports FR-015, FR-016, FR-017: Social Interactions - Comments
/// </summary>
public class Comment : BaseEntity
{
    public int UserId { get; set; }
    public int PostId { get; set; }
    public int? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? MentionedUserIds { get; set; } // JSON array of user IDs

    // Navigation Properties
    public User User { get; set; } = null!;
    public Post Post { get; set; } = null!;
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();

    // Domain Methods
    public void UpdateContent(string newContent)
    {
        Content = newContent;
        UpdateTimestamp();
    }

    public void AddMentions(List<int> userIds)
    {
        if (userIds.Any())
        {
            MentionedUserIds = JsonSerializer.Serialize(userIds);
            UpdateTimestamp();
        }
    }

    public List<int> GetMentionedUserIds()
    {
        if (string.IsNullOrWhiteSpace(MentionedUserIds))
            return new List<int>();

        try
        {
            return JsonSerializer.Deserialize<List<int>>(MentionedUserIds) ?? new List<int>();
        }
        catch
        {
            return new List<int>();
        }
    }

    public bool IsReply()
    {
        return ParentCommentId.HasValue;
    }

    public bool HasMentions()
    {
        return !string.IsNullOrWhiteSpace(MentionedUserIds);
    }

    public int GetReplyCount()
    {
        return Replies.Count;
    }
}