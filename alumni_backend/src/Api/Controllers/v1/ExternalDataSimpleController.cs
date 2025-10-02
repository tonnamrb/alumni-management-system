using Api.Helpers;
using Application.DTOs;
using Application.DTOs.ExternalData;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

/// <summary>
/// API Controller สำหรับจัดการข้อมูล Alumni จากระบบภายนอก (Simplified Version)
/// </summary>
[ApiController]
[Route("api/v1/external-data")]
[Authorize(Roles = "Admin,SystemIntegrator")]
[Produces("application/json")]
public class ExternalDataSimpleController : ControllerBase
{
    private readonly IExternalDataIntegrationService _integrationService;
    private readonly ILogger<ExternalDataSimpleController> _logger;

    public ExternalDataSimpleController(
        IExternalDataIntegrationService integrationService,
        ILogger<ExternalDataSimpleController> logger)
    {
        _integrationService = integrationService;
        _logger = logger;
    }

    /// <summary>
    /// นำเข้าข้อมูล Alumni จากระบบภายนอกแบบ Bulk Import
    /// </summary>
    [HttpPost("bulk-import")]
    [ProducesResponseType(typeof(ApiResponseDto<ImportResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkImport([FromBody] BulkImportRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ApiResponseHelper.BadRequest("Invalid request data");
            }

            if (request.Alumni == null || !request.Alumni.Any())
            {
                return ApiResponseHelper.BadRequest("No alumni data provided");
            }

            var result = await _integrationService.ProcessBulkDataAsync(request);
            return ApiResponseHelper.Success(result, "Bulk import completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during bulk import");
            return ApiResponseHelper.InternalServerError("Internal server error during bulk import");
        }
    }

    /// <summary>
    /// ตรวจสอบข้อมูล Alumni ก่อนนำเข้าจริง
    /// </summary>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ApiResponseDto<ValidationResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidateData([FromBody] BulkImportRequest request)
    {
        try
        {
            if (request.Alumni == null || !request.Alumni.Any())
            {
                return ApiResponseHelper.BadRequest("No alumni data provided");
            }

            var result = await _integrationService.ValidateDataAsync(request.Alumni, request.ExternalSystemId);
            return ApiResponseHelper.Success(result, "Validation completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during validation");
            return ApiResponseHelper.InternalServerError("Internal server error during validation");
        }
    }

    /// <summary>
    /// ซิงค์ข้อมูล Alumni รายคน
    /// </summary>
    [HttpPost("sync-single")]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SyncSingle([FromBody] ExternalAlumniData data, [FromQuery] string externalSystemId)
    {
        try
        {
            var result = await _integrationService.SyncSingleRecordAsync(data, externalSystemId);
            return ApiResponseHelper.Success(result, result ? "Record synced successfully" : "Failed to sync record");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing single record");
            return ApiResponseHelper.InternalServerError("Internal server error during sync");
        }
    }

    /// <summary>
    /// ดึงสถิติการนำเข้าข้อมูล
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ApiResponseDto<ImportStatistics>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatistics([FromQuery] string? externalSystemId = null)
    {
        try
        {
            var statistics = await _integrationService.GetImportStatisticsAsync(externalSystemId);
            return ApiResponseHelper.Success(statistics, "Statistics retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving statistics");
            return ApiResponseHelper.InternalServerError("Internal server error retrieving statistics");
        }
    }
}