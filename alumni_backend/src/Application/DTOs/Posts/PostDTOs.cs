using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Posts;

public class CreatePostDto
{
    [Required(ErrorMessage = "Content is required")]
    [StringLength(2000, ErrorMessage = "Content must be between 1 and 2000 characters", MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;
    
    public PostType Type { get; set; } = PostType.Text;
    
    [Url(ErrorMessage = "Invalid image URL format")]
    public string? ImageUrl { get; set; }
    
    public List<string>? MediaUrls { get; set; }
}

public class UpdatePostDto
{
    [Required(ErrorMessage = "Content is required")]
    [StringLength(2000, ErrorMessage = "Content must be between 1 and 2000 characters", MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;
    
    public PostType Type { get; set; } = PostType.Text;
    
    [Url(ErrorMessage = "Invalid image URL format")]
    public string? ImageUrl { get; set; }
    
    public List<string>? MediaUrls { get; set; }
}

public class PostDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserAvatar { get; set; }
    public string Content { get; set; } = string.Empty;
    public PostType Type { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> MediaUrls { get; set; } = new();
    public int MediaCount { get; set; }
    public bool IsPinned { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
    public bool IsOwnPost { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PostListDto
{
    public List<PostDto> Posts { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}