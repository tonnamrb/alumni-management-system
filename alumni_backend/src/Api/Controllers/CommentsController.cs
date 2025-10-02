using Application.DTOs;
using Application.DTOs.Comments;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Comments management endpoints - Phase 2: Moderation Features
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class CommentsController : BaseController
{
    private readonly ICommentService _commentService;
    private readonly ILogger<CommentsController> _logger;

    public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    /// <summary>
    /// Get comments for a post with pagination
    /// </summary>
    [HttpGet("post/{postId}")]
    [ProducesResponseType(typeof(ApiResponseDto<CommentListDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    public async Task<ActionResult<ApiResponseDto<CommentListDto>>> GetComments(
        int postId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var comments = await _commentService.GetCommentsAsync(postId, page, pageSize, currentUserId);
            return Ok(SuccessResponse(comments, $"Retrieved {comments.Comments.Count} comments"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comments for post {PostId}", postId);
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการดึงคอมเมนต์"));
        }
    }

    /// <summary>
    /// Get a specific comment by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseDto<CommentDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    public async Task<ActionResult<ApiResponseDto<CommentDto>>> GetComment(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var comment = await _commentService.GetCommentByIdAsync(id, currentUserId);
            
            if (comment == null)
                return NotFoundResponse("Comment not found");

            return Ok(SuccessResponse(comment, "Comment retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comment {CommentId}", id);
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการดึงคอมเมนต์"));
        }
    }

    /// <summary>
    /// Create a new comment
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponseDto<CommentDto>), 201)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    public async Task<ActionResult<ApiResponseDto<CommentDto>>> CreateComment([FromBody] CreateCommentDto createCommentDto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return UnauthorizedResponse("User authentication required");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponse<object>("Invalid comment data"));
            }

            var comment = await _commentService.CreateCommentAsync(currentUserId.Value, createCommentDto);
            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, 
                SuccessResponse(comment, "Comment created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ErrorResponse<object>(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating comment");
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการสร้างคอมเมนต์"));
        }
    }

    /// <summary>
    /// Update a comment (owner only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponseDto<CommentDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 403)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    public async Task<ActionResult<ApiResponseDto<CommentDto>>> UpdateComment(int id, [FromBody] UpdateCommentDto updateCommentDto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return UnauthorizedResponse("User authentication required");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponse<object>("Invalid comment data"));
            }

            var comment = await _commentService.UpdateCommentAsync(id, currentUserId.Value, updateCommentDto);
            return Ok(SuccessResponse(comment, "Comment updated successfully"));
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
            _logger.LogError(ex, "Error updating comment {CommentId}", id);
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการแก้ไขคอมเมนต์"));
        }
    }

    /// <summary>
    /// Delete a comment (owner only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 403)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    public async Task<ActionResult<ApiResponseDto<object>>> DeleteComment(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return UnauthorizedResponse("User authentication required");
            }

            var success = await _commentService.DeleteCommentAsync(id, currentUserId.Value);

            if (!success)
            {
                return NotFoundResponse("Comment not found");
            }

            return Ok(SuccessResponse<object>(null, "Comment deleted successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return ForbiddenResponse(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment {CommentId}", id);
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการลบคอมเมนต์"));
        }
    }

    /// <summary>
    /// Like/Unlike a comment
    /// </summary>
    [HttpPost("{id}/like")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponseDto<CommentDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    public async Task<ActionResult<ApiResponseDto<CommentDto>>> ToggleLike(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return UnauthorizedResponse("User authentication required");
            }

            var comment = await _commentService.ToggleCommentLikeAsync(id, currentUserId.Value);
            return Ok(SuccessResponse(comment, "Comment like toggled successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return NotFoundResponse(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling like for comment {CommentId}", id);
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการกดไลค์คอมเมนต์"));
        }
    }

    /// <summary>
    /// Get replies for a comment
    /// </summary>
    [HttpGet("{id}/replies")]
    [ProducesResponseType(typeof(ApiResponseDto<List<CommentDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    public async Task<ActionResult<ApiResponseDto<List<CommentDto>>>> GetReplies(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var replies = await _commentService.GetRepliesAsync(id, currentUserId);
            return Ok(SuccessResponse(replies, $"Retrieved {replies.Count} replies"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting replies for comment {CommentId}", id);
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการดึงการตอบกลับ"));
        }
    }
}