using Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// ดึง User ID จาก JWT token ใน current request
    /// </summary>
    protected int? GetCurrentUserId()
    {
        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User?.FindFirst("user_id")?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    /// <summary>
    /// ดึง User Email จาก JWT token ใน current request
    /// </summary>
    protected string? GetCurrentUserEmail()
    {
        return User?.FindFirst(ClaimTypes.Email)?.Value;
    }

    /// <summary>
    /// ดึง User Role จาก JWT token ใน current request
    /// </summary>
    protected string? GetCurrentUserRole()
    {
        return User?.FindFirst(ClaimTypes.Role)?.Value;
    }

    /// <summary>
    /// ตรวจสอบว่า current user เป็น Admin หรือไม่
    /// </summary>
    protected bool IsCurrentUserAdmin()
    {
        return GetCurrentUserRole() == "Admin";
    }

    /// <summary>
    /// ดึง IP Address ของ client
    /// </summary>
    protected string? GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    /// <summary>
    /// ดึง User Agent ของ client
    /// </summary>
    protected string? GetClientUserAgent()
    {
        return HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
    }

    /// <summary>
    /// สร้าง ApiResponse สำหรับ success case
    /// </summary>
    protected ApiResponseDto<T> SuccessResponse<T>(T data, string? message = null)
    {
        return new ApiResponseDto<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    /// <summary>
    /// สร้าง ApiResponse สำหรับ error case
    /// </summary>
    protected ApiResponseDto<T> ErrorResponse<T>(string error, string? message = null)
    {
        return new ApiResponseDto<T>
        {
            Success = false,
            Error = error,
            Message = message
        };
    }

    /// <summary>
    /// สร้าง BadRequest response พร้อม validation errors
    /// </summary>
    protected BadRequestObjectResult ValidationErrorResponse(string message, List<ValidationErrorDto>? errors = null)
    {
        var response = ErrorResponse<object>(message);
        
        if (errors?.Any() == true)
        {
            return BadRequest(new
            {
                response.Success,
                response.Error,
                response.Message,
                ValidationErrors = errors
            });
        }

        return BadRequest(response);
    }

    /// <summary>
    /// สร้าง NotFound response
    /// </summary>
    protected NotFoundObjectResult NotFoundResponse(string message = "ไม่พบข้อมูลที่ต้องการ")
    {
        return NotFound(ErrorResponse<object>(message));
    }

    /// <summary>
    /// สร้าง Unauthorized response
    /// </summary>
    protected UnauthorizedObjectResult UnauthorizedResponse(string message = "ไม่มีสิทธิ์เข้าถึง")
    {
        return Unauthorized(ErrorResponse<object>(message));
    }

    /// <summary>
    /// สร้าง Forbidden response
    /// </summary>
    protected ObjectResult ForbiddenResponse(string message = "ไม่ได้รับอนุญาตให้เข้าถึง")
    {
        return StatusCode(403, ErrorResponse<object>(message));
    }

    /// <summary>
    /// ตรวจสอบว่าผู้ใช้งานปัจจุบันเป็น Admin หรือ Moderator หรือไม่
    /// </summary>
    protected bool IsCurrentUserAdminOrModerator()
    {
        var userRole = GetCurrentUserRole();
        return userRole == "Admin" || userRole == "Moderator";
    }
}