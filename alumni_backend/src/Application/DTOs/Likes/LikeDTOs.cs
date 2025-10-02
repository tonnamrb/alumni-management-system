using Application.DTOs;

namespace Application.DTOs.Likes;

public class LikeDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? PostId { get; set; }
    public int? CommentId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation Properties
    public UserDto User { get; set; } = new();
}