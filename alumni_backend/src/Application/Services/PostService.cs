using Application.DTOs.Posts;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly ILikeRepository _likeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PostService> _logger;

    public PostService(
        IPostRepository postRepository,
        ILikeRepository likeRepository,
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<PostService> logger)
    {
        _postRepository = postRepository;
        _likeRepository = likeRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PostListDto> GetPostsAsync(int page = 1, int pageSize = 10, int? currentUserId = null, PostType? type = null)
    {
        try
        {
            // Get pinned posts first, then regular posts (filtered by type if specified)
            var pinnedPosts = await _postRepository.GetPinnedPostsAsync(type);
            var regularPosts = await _postRepository.GetPostsWithUserAndLikesAsync(page, pageSize, type);
            var totalCount = await _postRepository.GetPostsCountAsync(type);

            var allPosts = new List<Post>();
            
            // Add pinned posts at top (only on first page)
            if (page == 1)
            {
                allPosts.AddRange(pinnedPosts.OrderByDescending(p => p.CreatedAt));
            }
            
            // Add regular posts
            allPosts.AddRange(regularPosts.Where(p => !p.IsPinned));

            var postDtos = new List<PostDto>();
            foreach (var post in allPosts)
            {
                var dto = await MapToPostDtoAsync(post, currentUserId);
                postDtos.Add(dto);
            }

            return new PostListDto
            {
                Posts = postDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                HasNextPage = page * pageSize < totalCount,
                HasPreviousPage = page > 1
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting posts for page {Page}", page);
            throw;
        }
    }

    public async Task<PostDto?> GetPostByIdAsync(int postId, int? currentUserId = null)
    {
        try
        {
            var post = await _postRepository.GetByIdWithUserAsync(postId);
            if (post == null) return null;

            return await MapToPostDtoAsync(post, currentUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting post {PostId}", postId);
            throw;
        }
    }

    public async Task<PostDto> CreatePostAsync(int userId, CreatePostDto createPostDto)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            var post = new Post
            {
                UserId = userId,
                Content = createPostDto.Content.Trim(),
                Type = createPostDto.Type,
                CreatedAt = DateTime.UtcNow
            };

            // Handle media URLs based on type and count
            if (createPostDto.MediaUrls?.Count > 0)
            {
                post.AttachMultipleMedia(createPostDto.MediaUrls, createPostDto.Type);
            }
            else if (!string.IsNullOrEmpty(createPostDto.ImageUrl))
            {
                post.AttachImage(createPostDto.ImageUrl.Trim());
            }

            var createdPost = await _postRepository.AddAsync(post);
            await _postRepository.SaveChangesAsync();

            _logger.LogInformation("Post created by user {UserId}: {PostId}", userId, createdPost.Id);
            
            return await MapToPostDtoAsync(createdPost, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating post for user {UserId}", userId);
            throw;
        }
    }

    public async Task<PostDto> UpdatePostAsync(int postId, int userId, UpdatePostDto updatePostDto)
    {
        try
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                throw new InvalidOperationException("Post not found");

            if (post.UserId != userId)
                throw new UnauthorizedAccessException("You can only update your own posts");

            post.UpdateContent(updatePostDto.Content.Trim());
            post.Type = updatePostDto.Type;
            
            // Handle media updates
            if (updatePostDto.MediaUrls?.Count > 0)
            {
                post.AttachMultipleMedia(updatePostDto.MediaUrls, updatePostDto.Type);
            }
            else if (!string.IsNullOrEmpty(updatePostDto.ImageUrl))
            {
                post.AttachImage(updatePostDto.ImageUrl.Trim());
            }
            else
            {
                post.RemoveImage();
            }

            await _postRepository.UpdateAsync(post);
            await _postRepository.SaveChangesAsync();

            _logger.LogInformation("Post {PostId} updated by user {UserId}", postId, userId);
            
            return await MapToPostDtoAsync(post, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating post {PostId} by user {UserId}", postId, userId);
            throw;
        }
    }

    public async Task<bool> DeletePostAsync(int postId, int userId, bool isAdmin = false)
    {
        try
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                return false;

            // Check permissions
            if (!isAdmin && post.UserId != userId)
                throw new UnauthorizedAccessException("You can only delete your own posts");

            await _postRepository.DeleteAsync(post);
            await _postRepository.SaveChangesAsync();

            _logger.LogInformation("Post {PostId} deleted by user {UserId} (Admin: {IsAdmin})", 
                postId, userId, isAdmin);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting post {PostId} by user {UserId}", postId, userId);
            throw;
        }
    }

    public async Task<bool> ToggleLikeAsync(int postId, int userId)
    {
        try
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                throw new InvalidOperationException("Post not found");

            var existingLike = await _likeRepository.GetLikeAsync(postId, userId);
            
            if (existingLike != null)
            {
                // Unlike
                await _likeRepository.DeleteAsync(existingLike);
                await _likeRepository.SaveChangesAsync();
                
                _logger.LogInformation("User {UserId} unliked post {PostId}", userId, postId);
                return false;
            }
            else
            {
                // Like
                var like = Like.Create(userId, postId);
                await _likeRepository.AddAsync(like);
                await _likeRepository.SaveChangesAsync();
                
                _logger.LogInformation("User {UserId} liked post {PostId}", userId, postId);
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling like for post {PostId} by user {UserId}", postId, userId);
            throw;
        }
    }

    public async Task<int> GetLikesCountAsync(int postId)
    {
        return await _postRepository.GetLikesCountAsync(postId);
    }

    public async Task<bool> IsLikedByUserAsync(int postId, int userId)
    {
        return await _postRepository.IsLikedByUserAsync(postId, userId);
    }

    public async Task<PostDto> PinPostAsync(int postId, int adminUserId)
    {
        try
        {
            // Check if admin
            var admin = await _userRepository.GetByIdAsync(adminUserId);
            if (admin?.RoleId != 2) // RoleId 2 = Administrator
                throw new UnauthorizedAccessException("Only admins can pin posts");

            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                throw new InvalidOperationException("Post not found");

            if (post.IsPinned)
                throw new InvalidOperationException("Post is already pinned");

            // Check pinned posts limit (max 5)
            var pinnedCount = await _postRepository.GetPinnedPostsCountAsync();
            if (pinnedCount >= 5)
                throw new InvalidOperationException("Maximum 5 posts can be pinned at once");

            post.Pin();
            await _postRepository.UpdateAsync(post);
            await _postRepository.SaveChangesAsync();

            _logger.LogInformation("Post {PostId} pinned by admin {AdminId}", postId, adminUserId);
            
            return await MapToPostDtoAsync(post, adminUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pinning post {PostId} by admin {AdminId}", postId, adminUserId);
            throw;
        }
    }

    public async Task<PostDto> UnpinPostAsync(int postId, int adminUserId)
    {
        try
        {
            // Check if admin
            var admin = await _userRepository.GetByIdAsync(adminUserId);
            if (admin?.RoleId != 2) // RoleId 2 = Administrator
                throw new UnauthorizedAccessException("Only admins can unpin posts");

            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                throw new InvalidOperationException("Post not found");

            if (!post.IsPinned)
                throw new InvalidOperationException("Post is not pinned");

            post.Unpin();
            await _postRepository.UpdateAsync(post);
            await _postRepository.SaveChangesAsync();

            _logger.LogInformation("Post {PostId} unpinned by admin {AdminId}", postId, adminUserId);
            
            return await MapToPostDtoAsync(post, adminUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unpinning post {PostId} by admin {AdminId}", postId, adminUserId);
            throw;
        }
    }

    public async Task<List<PostDto>> GetPinnedPostsAsync()
    {
        try
        {
            var pinnedPosts = await _postRepository.GetPinnedPostsAsync();
            var postDtos = new List<PostDto>();

            foreach (var post in pinnedPosts.OrderByDescending(p => p.CreatedAt))
            {
                var dto = await MapToPostDtoAsync(post, null);
                postDtos.Add(dto);
            }

            return postDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pinned posts");
            throw;
        }
    }

    private async Task<PostDto> MapToPostDtoAsync(Post post, int? currentUserId)
    {
        var likesCount = await GetLikesCountAsync(post.Id);
        var isLiked = currentUserId.HasValue ? await IsLikedByUserAsync(post.Id, currentUserId.Value) : false;
        var isOwnPost = currentUserId.HasValue && post.UserId == currentUserId.Value;

        return new PostDto
        {
            Id = post.Id,
            UserId = post.UserId,
            UserName = post.User?.FullName ?? "Unknown",
            UserAvatar = post.User?.AlumniProfile?.ProfilePictureUrl,
            Content = post.Content,
            Type = post.Type,
            ImageUrl = post.ImageUrl,
            MediaUrls = post.GetMediaUrls(),
            MediaCount = post.GetMediaCount(),
            IsPinned = post.IsPinned,
            LikesCount = likesCount,
            CommentsCount = post.Comments?.Count ?? 0,
            IsLikedByCurrentUser = isLiked,
            IsOwnPost = isOwnPost,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
}
