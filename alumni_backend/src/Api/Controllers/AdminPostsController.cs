using Application.DTOs;
using Application.DTOs.Posts;
using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Admin posts management endpoints
/// </summary>
[ApiController]
[Route("api/v1/admin/posts")]
[Authorize(Roles = nameof(UserRole.Administrator))]
public class AdminPostsController : BaseController
{
    private readonly IPostService _postService;
    private readonly ILogger<AdminPostsController> _logger;

    public AdminPostsController(IPostService postService, ILogger<AdminPostsController> logger)
    {
        _postService = postService;
        _logger = logger;
    }

    /// <summary>
    /// Pin post (Admin only) - Maximum 5 pinned posts
    /// </summary>
    [HttpPost("{id}/pin")]
    [ProducesResponseType(typeof(ApiResponseDto<PostDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 403)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    public async Task<ActionResult<ApiResponseDto<PostDto>>> PinPost(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return UnauthorizedResponse("Admin authentication required");
            }

            var post = await _postService.PinPostAsync(id, currentUserId.Value);
            return Ok(SuccessResponse(post, "Post pinned successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return ForbiddenResponse(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ErrorResponse<object>(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pinning post {PostId}", id);
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการปักหมุดโพสต์"));
        }
    }

    /// <summary>
    /// Unpin post (Admin only)
    /// </summary>
    [HttpDelete("{id}/pin")]
    [ProducesResponseType(typeof(ApiResponseDto<PostDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 403)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    public async Task<ActionResult<ApiResponseDto<PostDto>>> UnpinPost(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return UnauthorizedResponse("Admin authentication required");
            }

            var post = await _postService.UnpinPostAsync(id, currentUserId.Value);
            return Ok(SuccessResponse(post, "Post unpinned successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return ForbiddenResponse(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ErrorResponse<object>(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unpinning post {PostId}", id);
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการเลิกปักหมุดโพสต์"));
        }
    }

    /// <summary>
    /// Get all pinned posts (Admin only)
    /// </summary>
    [HttpGet("pinned")]
    [ProducesResponseType(typeof(ApiResponseDto<List<PostDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 403)]
    public async Task<ActionResult<ApiResponseDto<List<PostDto>>>> GetPinnedPosts()
    {
        try
        {
            var pinnedPosts = await _postService.GetPinnedPostsAsync();
            return Ok(SuccessResponse(pinnedPosts, $"Retrieved {pinnedPosts.Count} pinned posts"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pinned posts");
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการดึงโพสต์ที่ปักหมุด"));
        }
    }

    /// <summary>
    /// Delete any post (Admin only)
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
                return UnauthorizedResponse("Admin authentication required");
            }

            var success = await _postService.DeletePostAsync(id, currentUserId.Value, isAdmin: true);

            if (!success)
            {
                return NotFoundResponse("Post not found");
            }

            return Ok(SuccessResponse<object>(null, "Post deleted successfully by admin"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting post {PostId} by admin", id);
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการลบโพสต์"));
        }
    }
}