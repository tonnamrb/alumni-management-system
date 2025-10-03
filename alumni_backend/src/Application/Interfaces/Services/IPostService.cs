using Application.DTOs.Posts;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface IPostService
{
    // Posts CRUD
    Task<PostListDto> GetPostsAsync(int page = 1, int pageSize = 10, int? currentUserId = null, PostType? type = null);
    Task<PostDto?> GetPostByIdAsync(int postId, int? currentUserId = null);
    Task<PostDto> CreatePostAsync(int userId, CreatePostDto createPostDto);
    Task<PostDto> UpdatePostAsync(int postId, int userId, UpdatePostDto updatePostDto);
    Task<bool> DeletePostAsync(int postId, int userId, bool isAdmin = false);
    
    // Likes
    Task<bool> ToggleLikeAsync(int postId, int userId);
    Task<int> GetLikesCountAsync(int postId);
    Task<bool> IsLikedByUserAsync(int postId, int userId);
    
    // Admin features
    Task<PostDto> PinPostAsync(int postId, int adminUserId);
    Task<PostDto> UnpinPostAsync(int postId, int adminUserId);
    Task<List<PostDto>> GetPinnedPostsAsync();
}