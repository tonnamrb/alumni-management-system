using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// File upload management endpoints
/// </summary>
[ApiController]
[Route("api/v1/upload")]
[Authorize]
public class UploadController : BaseController
{
    private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB
    private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    /// <summary>
    /// อัพโหลดรูปภาพ
    /// </summary>
    [HttpPost("image")]
    [ProducesResponseType(typeof(ApiResponseDto<FileUploadResultDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    public async Task<ActionResult<ApiResponseDto<FileUploadResultDto>>> UploadImage(
        IFormFile file,
        [FromForm] string folder = "general")
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return UnauthorizedResponse();
            }

            // ตรวจสอบไฟล์
            var validationResult = ValidateImageFile(file);
            if (!validationResult.IsValid)
            {
                return BadRequest(ErrorResponse<object>(validationResult.ErrorMessage));
            }

            // Mock response สำหรับตอนนี้
            var mockResult = new FileUploadResultDto
            {
                FileName = file.FileName,
                FileUrl = $"https://mock-storage.com/{folder}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}",
                FileSize = file.Length,
                ContentType = file.ContentType,
                UploadedAt = DateTime.UtcNow
            };

            return Ok(SuccessResponse(mockResult, "อัพโหลดรูปภาพสำเร็จ"));
        }
        catch (Exception ex)
        {
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการอัพโหลดรูปภาพ"));
        }
    }

    /// <summary>
    /// ดึงรายการไฟล์ของผู้ใช้งาน
    /// </summary>
    [HttpGet("my-files")]
    [ProducesResponseType(typeof(ApiResponseDto<List<UserFileDto>>), 200)]
    public async Task<ActionResult<ApiResponseDto<List<UserFileDto>>>> GetMyFiles(
        [FromQuery] string? folder = null)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return UnauthorizedResponse();
            }

            // Mock data สำหรับตอนนี้
            var mockFiles = new List<UserFileDto>
            {
                new UserFileDto
                {
                    Id = 1,
                    FileName = "profile.jpg",
                    FileUrl = "https://mock-storage.com/profiles/profile.jpg",
                    FileSize = 1024000,
                    ContentType = "image/jpeg",
                    UploadedAt = DateTime.UtcNow.AddDays(-1),
                    Folder = "profiles"
                }
            };

            return Ok(SuccessResponse(mockFiles));
        }
        catch (Exception ex)
        {
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการดึงข้อมูลไฟล์"));
        }
    }

    /// <summary>
    /// ตรวจสอบไฟล์รูปภาพ
    /// </summary>
    private (bool IsValid, string ErrorMessage) ValidateImageFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return (false, "ไม่พบไฟล์ที่ต้องการอัพโหลด");
        }

        if (file.Length > _maxFileSize)
        {
            return (false, $"ขนาดไฟล์ใหญ่เกินไป (สูงสุด {_maxFileSize / 1024 / 1024} MB)");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedImageExtensions.Contains(extension))
        {
            return (false, $"ประเภทไฟล์ไม่ถูกต้อง อนุญาตเฉพาะ: {string.Join(", ", _allowedImageExtensions)}");
        }

        return (true, string.Empty);
    }
}