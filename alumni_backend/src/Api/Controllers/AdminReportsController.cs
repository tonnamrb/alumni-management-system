using Application.DTOs;
using Application.DTOs.Reports;
using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/admin/reports")]
[ApiVersion("1.0")]
[Authorize(Roles = "Administrator")]
public class AdminReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly ILogger<AdminReportsController> _logger;

    public AdminReportsController(
        IReportService reportService,
        ILogger<AdminReportsController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    /// <summary>
    /// ดูรายงานทั้งหมด (สำหรับแอดมิน)
    /// </summary>
    /// <param name="status">สถานะของรายงาน</param>
    /// <param name="type">ประเภทของรายงาน</param>
    /// <param name="page">หน้าที่ต้องการ</param>
    /// <param name="pageSize">จำนวนรายการต่อหน้า (มากสุด 50)</param>
    /// <returns>รายการรายงานทั้งหมด</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<ReportListDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 403)]
    public async Task<IActionResult> GetAllReports(
        [FromQuery] ReportStatus? status = null,
        [FromQuery] ReportType? type = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var reports = await _reportService.GetAllReportsAsync(status, type, page, pageSize);

            return Ok(new ApiResponseDto<ReportListDto>
            {
                Success = true,
                Data = reports
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all reports for admin");
            return StatusCode(500, new ApiResponseDto<object>
            {
                Success = false,
                Error = "Internal server error"
            });
        }
    }

    /// <summary>
    /// ดูรายงานตาม ID (สำหรับแอดมิน)
    /// </summary>
    /// <param name="reportId">ID ของรายงาน</param>
    /// <returns>ข้อมูลรายงาน</returns>
    [HttpGet("{reportId}")]
    [ProducesResponseType(typeof(ApiResponseDto<ReportDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 403)]
    public async Task<IActionResult> GetReport(int reportId)
    {
        try
        {
            var report = await _reportService.GetReportByIdAsync(reportId);

            if (report == null)
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Error = "Report not found"
                });

            return Ok(new ApiResponseDto<ReportDto>
            {
                Success = true,
                Data = report
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting report {ReportId} for admin", reportId);
            return StatusCode(500, new ApiResponseDto<object>
            {
                Success = false,
                Error = "Internal server error"
            });
        }
    }

    /// <summary>
    /// แก้ไขสถานะรายงาน (ปิดเคส)
    /// </summary>
    /// <param name="reportId">ID ของรายงาน</param>
    /// <param name="resolveReportDto">ข้อมูลการปิดเคส</param>
    /// <returns>ข้อมูลรายงานที่อัปเดตแล้ว</returns>
    [HttpPut("{reportId}/resolve")]
    [ProducesResponseType(typeof(ApiResponseDto<ReportDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 403)]
    public async Task<IActionResult> ResolveReport(int reportId, [FromBody] ResolveReportDto resolveReportDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Error = "Invalid data"
                });

            var adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var report = await _reportService.ResolveReportAsync(reportId, adminUserId, resolveReportDto);

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
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new ApiResponseDto<object>
            {
                Success = false,
                Error = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving report {ReportId}", reportId);
            return StatusCode(500, new ApiResponseDto<object>
            {
                Success = false,
                Error = "Internal server error"
            });
        }
    }

    /// <summary>
    /// ดูสถิติรายงานตามสถานะ
    /// </summary>
    /// <returns>สถิติรายงาน</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ApiResponseDto<Dictionary<string, int>>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 403)]
    public async Task<IActionResult> GetReportStatistics()
    {
        try
        {
            var statistics = await _reportService.GetReportStatisticsAsync();

            // แปลงจาก ReportStatus enum เป็น string
            var stringStatistics = statistics.ToDictionary(
                x => x.Key.ToString(),
                x => x.Value
            );

            return Ok(new ApiResponseDto<Dictionary<string, int>>
            {
                Success = true,
                Data = stringStatistics
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting report statistics");
            return StatusCode(500, new ApiResponseDto<object>
            {
                Success = false,
                Error = "Internal server error"
            });
        }
    }

    /// <summary>
    /// ดูสถิติรายงานตามประเภท
    /// </summary>
    /// <returns>สถิติรายงานตามประเภท</returns>
    [HttpGet("statistics/types")]
    [ProducesResponseType(typeof(ApiResponseDto<Dictionary<string, int>>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 403)]
    public async Task<IActionResult> GetReportTypeStatistics()
    {
        try
        {
            var statistics = await _reportService.GetReportTypeStatisticsAsync();

            // แปลงจาก ReportType enum เป็น string
            var stringStatistics = statistics.ToDictionary(
                x => x.Key.ToString(),
                x => x.Value
            );

            return Ok(new ApiResponseDto<Dictionary<string, int>>
            {
                Success = true,
                Data = stringStatistics
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting report type statistics");
            return StatusCode(500, new ApiResponseDto<object>
            {
                Success = false,
                Error = "Internal server error"
            });
        }
    }

    /// <summary>
    /// ดูสถิติรายงานแบบสรุป
    /// </summary>
    /// <returns>สถิติรายงานแบบสรุป</returns>
    [HttpGet("statistics/summary")]
    [ProducesResponseType(typeof(ApiResponseDto<AdminReportSummaryDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 403)]
    public async Task<IActionResult> GetReportSummary()
    {
        try
        {
            var statusStatistics = await _reportService.GetReportStatisticsAsync();
            var typeStatistics = await _reportService.GetReportTypeStatisticsAsync();

            var summary = new AdminReportSummaryDto
            {
                StatusStatistics = statusStatistics.ToDictionary(x => x.Key.ToString(), x => x.Value),
                TypeStatistics = typeStatistics.ToDictionary(x => x.Key.ToString(), x => x.Value),
                TotalReports = statusStatistics.Values.Sum(),
                PendingReports = statusStatistics.GetValueOrDefault(ReportStatus.Pending, 0),
                ResolvedReports = statusStatistics.GetValueOrDefault(ReportStatus.Resolved, 0),
                DismissedReports = statusStatistics.GetValueOrDefault(ReportStatus.Dismissed, 0)
            };

            return Ok(new ApiResponseDto<AdminReportSummaryDto>
            {
                Success = true,
                Data = summary
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting report summary");
            return StatusCode(500, new ApiResponseDto<object>
            {
                Success = false,
                Error = "Internal server error"
            });
        }
    }
}



public class AdminReportSummaryDto
{
    public Dictionary<string, int> StatusStatistics { get; set; } = new();
    public Dictionary<string, int> TypeStatistics { get; set; } = new();
    public int TotalReports { get; set; }
    public int PendingReports { get; set; }
    public int ResolvedReports { get; set; }
    public int DismissedReports { get; set; }
}
