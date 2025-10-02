using Application.DTOs;
using Application.DTOs.Posts;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Posts management endpoints - Phase 1: Core Social Features
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PostsController : BaseController
{
    private readonly IPostService _postService;
    private readonly ILogger<PostsController> _logger;

    public PostsController(IPostService postService, ILogger<PostsController> logger)
    {
        _postService = postService;
        _logger = logger;
    }

    /// <summary>
    /// Get posts feed with pagination (pinned posts appear first on page 1)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponseDto<PostListDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    public async Task<ActionResult<ApiResponseDto<PostListDto>>> GetPosts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page < 1 || pageSize < 1 || pageSize > 50)
            {
                return BadRequest(ErrorResponse<object>("Invalid pagination parameters"));
            }

            var currentUserId = GetCurrentUserId();
            var posts = await _postService.GetPostsAsync(page, pageSize, currentUserId);

            return Ok(SuccessResponse(posts, "Posts retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting posts");
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการดึงข้อมูลโพสต์"));
        }
    }

    /// <summary>
    /// Get post by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponseDto<PostDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    public async Task<ActionResult<ApiResponseDto<PostDto>>> GetPost(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var post = await _postService.GetPostByIdAsync(id, currentUserId);

            if (post == null)
            {
                return NotFoundResponse("Post not found");
            }

            return Ok(SuccessResponse(post, "Post retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting post {PostId}", id);
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการดึงข้อมูลโพสต์"));
        }
    }

    /// <summary>
    /// Create new post
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseDto<PostDto>), 201)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    public async Task<ActionResult<ApiResponseDto<PostDto>>> CreatePost([FromBody] CreatePostDto createPostDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponse<object>("Invalid post data"));
            }

            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return UnauthorizedResponse("User not authenticated");
            }

            var post = await _postService.CreatePostAsync(currentUserId.Value, createPostDto);
            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, SuccessResponse(post, "Post created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating post");
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการสร้างโพสต์"));
        }
    }

    /// <summary>
    /// Update own post
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseDto<PostDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 403)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    public async Task<ActionResult<ApiResponseDto<PostDto>>> UpdatePost(int id, [FromBody] UpdatePostDto updatePostDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponse<object>("Invalid post data"));
            }

            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return UnauthorizedResponse("User not authenticated");
            }

            var post = await _postService.UpdatePostAsync(id, currentUserId.Value, updatePostDto);
            return Ok(SuccessResponse(post, "Post updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return NotFoundResponse(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return ForbiddenResponse(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating post {PostId}", id);
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการอัปเดตโพสต์"));
        }
    }

    /// <summary>
    /// Delete own post (users) or any post (admins)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 403)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    public async Task<ActionResult<ApiResponseDto<object>>> DeletePost(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return UnauthorizedResponse("User not authenticated");
            }

            var isAdmin = IsCurrentUserAdmin();
            var success = await _postService.DeletePostAsync(id, currentUserId.Value, isAdmin);

            if (!success)
            {
                return NotFoundResponse("Post not found");
            }

            return Ok(SuccessResponse<object>(null, "Post deleted successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return ForbiddenResponse(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting post {PostId}", id);
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการลบโพสต์"));
        }
    }

    /// <summary>
    /// Toggle like/unlike post
    /// </summary>
    [HttpPost("{id}/like")]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    public async Task<ActionResult<ApiResponseDto<object>>> ToggleLike(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return UnauthorizedResponse("User not authenticated");
            }

            var isLiked = await _postService.ToggleLikeAsync(id, currentUserId.Value);
            var message = isLiked ? "Post liked" : "Post unliked";
            
            return Ok(SuccessResponse(new { isLiked }, message));
        }
        catch (InvalidOperationException ex)
        {
            return NotFoundResponse(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling like for post {PostId}", id);
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการกด Like"));
        }
    }

    /// <summary>
    /// Get post likes count
    /// </summary>
    [HttpGet("{id}/likes/count")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 200)]
    public async Task<ActionResult<ApiResponseDto<object>>> GetLikesCount(int id)
    {
        try
        {
            var count = await _postService.GetLikesCountAsync(id);
            return Ok(SuccessResponse(new { count }, "Likes count retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting likes count for post {PostId}", id);
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการดึงจำนวน Like"));
        }
    }
}