using Application.DTOs;
using Application.DTOs.Reports;
using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(
        IReportService reportService,
        ILogger<ReportsController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    /// <summary>
    /// สร้างการรายงานใหม่
    /// </summary>
    /// <param name="createReportDto">ข้อมูลการรายงาน</param>
    /// <returns>ข้อมูลการรายงานที่สร้างใหม่</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseDto<ReportDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    public async Task<IActionResult> CreateReport([FromBody] CreateReportDto createReportDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Error = "Invalid data"
                });

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var report = await _reportService.CreateReportAsync(userId, createReportDto);

            return Ok(new ApiResponseDto<ReportDto>
            {
                Success = true,
                Data = report
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<object>
            {
                Success = false,
                Error = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating report");
            return StatusCode(500, new ApiResponseDto<object>
            {
                Success = false,
                Error = "Internal server error"
            });
        }
    }

    /// <summary>
    /// ดูรายงานของตัวเอง
    /// </summary>
    /// <param name="page">หน้าที่ต้องการ</param>
    /// <param name="pageSize">จำนวนรายการต่อหน้า (มากสุด 50)</param>
    /// <returns>รายการรายงานของผู้ใช้</returns>
    [HttpGet("my-reports")]
    [ProducesResponseType(typeof(ApiResponseDto<ReportListDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    public async Task<IActionResult> GetMyReports([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var reports = await _reportService.GetUserReportsAsync(userId, page, pageSize);

            return Ok(new ApiResponseDto<ReportListDto>
            {
                Success = true,
                Data = reports
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user reports");
            return StatusCode(500, new ApiResponseDto<object>
            {
                Success = false,
                Error = "Internal server error"
            });
        }
    }

    /// <summary>
    /// ดูรายงานตาม ID (สำหรับผู้รายงานหรือแอดมิน)
    /// </summary>
    /// <param name="reportId">ID ของรายงาน</param>
    /// <returns>ข้อมูลรายงาน</returns>
    [HttpGet("{reportId}")]
    [ProducesResponseType(typeof(ApiResponseDto<ReportDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 403)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    public async Task<IActionResult> GetReport(int reportId)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var report = await _reportService.GetReportByIdAsync(reportId);

            if (report == null)
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Error = "Report not found"
                });

            // ตรวจสอบสิทธิ์: เฉพาะผู้รายงานหรือแอดมินเท่านั้น
            if (report.ReporterId != userId && userRole != "Administrator")
            {
                return Forbid();
            }

            return Ok(new ApiResponseDto<ReportDto>
            {
                Success = true,
                Data = report
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting report {ReportId}", reportId);
            return StatusCode(500, new ApiResponseDto<object>
            {
                Success = false,
                Error = "Internal server error"
            });
        }
    }

    /// <summary>
    /// ตรวจสอบว่าเนื้อหาถูกรายงานหรือไม่
    /// </summary>
    /// <param name="postId">ID ของโพสต์ (ถ้ามี)</param>
    /// <param name="commentId">ID ของคอมเมนต์ (ถ้ามี)</param>
    /// <returns>ผลการตรวจสอบ</returns>
    [HttpGet("check-content")]
    [ProducesResponseType(typeof(ApiResponseDto<ContentReportStatusDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    public async Task<IActionResult> CheckContentReported([FromQuery] int? postId, [FromQuery] int? commentId)
    {
        try
        {
            if (!postId.HasValue && !commentId.HasValue)
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Error = "Either postId or commentId is required"
                });

            if (postId.HasValue && commentId.HasValue)
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Error = "Cannot check both postId and commentId at the same time"
                });

            var isReported = await _reportService.IsContentReportedAsync(postId, commentId);
            var reportCount = await _reportService.GetContentReportCountAsync(postId, commentId);

            var result = new ContentReportStatusDto
            {
                IsReported = isReported,
                ReportCount = reportCount,
                PostId = postId,
                CommentId = commentId
            };

            return Ok(new ApiResponseDto<ContentReportStatusDto>
            {
                Success = true,
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking content report status");
            return StatusCode(500, new ApiResponseDto<object>
            {
                Success = false,
                Error = "Internal server error"
            });
        }
    }
}



public class ContentReportStatusDto
{
    public bool IsReported { get; set; }
    public int ReportCount { get; set; }
    public int? PostId { get; set; }
    public int? CommentId { get; set; }
}
