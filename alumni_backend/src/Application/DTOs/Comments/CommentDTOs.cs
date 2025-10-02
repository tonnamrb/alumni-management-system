using System.ComponentModel.DataAnnotations;
using Application.DTOs;

namespace Application.DTOs.Comments;

public class CreateCommentDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Post ID จำเป็นต้องระบุ")]
    public int PostId { get; set; }
    
    public int? ParentCommentId { get; set; }
    
    [Required]
    [StringLength(1000, ErrorMessage = "เนื้อหาคอมเมนต์ต้องไม่เกิน 1000 ตัวอักษร")]
    public string Content { get; set; } = string.Empty;
    
    public List<int>? MentionedUserIds { get; set; }
}

public class UpdateCommentDto
{
    [Required]
    [StringLength(1000, ErrorMessage = "เนื้อหาคอมเมนต์ต้องไม่เกิน 1000 ตัวอักษร")]
    public string Content { get; set; } = string.Empty;
    
    public List<int>? MentionedUserIds { get; set; }
}

public class CommentDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PostId { get; set; }
    public int? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<int>? MentionedUserIds { get; set; }
    public int LikesCount { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation Properties  
    public UserDto User { get; set; } = new();
    public List<CommentDto>? Replies { get; set; }
    public List<UserDto>? MentionedUsers { get; set; }
}

public class CommentListDto
{
    public List<CommentDto> Comments { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}