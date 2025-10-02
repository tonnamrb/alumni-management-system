using Application.DTOs;
using Application.DTOs.Comments;
using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Admin comments management endpoints
/// </summary>
[ApiController]
[Route("api/v1/admin/comments")]
[Authorize(Roles = nameof(UserRole.Administrator))]
public class AdminCommentsController : BaseController
{
    private readonly ICommentService _commentService;
    private readonly ILogger<AdminCommentsController> _logger;

    public AdminCommentsController(ICommentService commentService, ILogger<AdminCommentsController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    /// <summary>
    /// Delete any comment (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
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
                return UnauthorizedResponse("Admin authentication required");
            }

            var success = await _commentService.DeleteCommentByAdminAsync(id, currentUserId.Value);

            if (!success)
            {
                return NotFoundResponse("Comment not found");
            }

            return Ok(SuccessResponse<object>(null, "Comment deleted successfully by admin"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return ForbiddenResponse(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment {CommentId} by admin", id);
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการลบคอมเมนต์"));
        }
    }

    /// <summary>
    /// Get reported comments (Admin only)
    /// </summary>
    [HttpGet("reported")]
    [ProducesResponseType(typeof(ApiResponseDto<CommentListDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 403)]
    public async Task<ActionResult<ApiResponseDto<CommentListDto>>> GetReportedComments(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var reportedComments = await _commentService.GetReportedCommentsAsync(page, pageSize);
            return Ok(SuccessResponse(reportedComments, $"Retrieved {reportedComments.Comments.Count} reported comments"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reported comments");
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการดึงคอมเมนต์ที่ถูกรายงาน"));
        }
    }
}