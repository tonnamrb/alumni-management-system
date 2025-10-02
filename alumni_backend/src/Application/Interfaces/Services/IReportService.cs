using Application.DTOs.Reports;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface IReportService
{
    // User Reports
    Task<ReportDto> CreateReportAsync(int reporterId, CreateReportDto createReportDto);
    Task<ReportListDto> GetUserReportsAsync(int userId, int page = 1, int pageSize = 10);
    
    // Admin Report Management
    Task<ReportListDto> GetAllReportsAsync(ReportStatus? status = null, ReportType? type = null, int page = 1, int pageSize = 10);
    Task<ReportDto?> GetReportByIdAsync(int reportId);
    Task<ReportDto> ResolveReportAsync(int reportId, int adminUserId, ResolveReportDto resolveReportDto);
    
    // Statistics
    Task<Dictionary<ReportStatus, int>> GetReportStatisticsAsync();
    Task<Dictionary<ReportType, int>> GetReportTypeStatisticsAsync();
    
    // Content Moderation
    Task<bool> IsContentReportedAsync(int? postId, int? commentId);
    Task<int> GetContentReportCountAsync(int? postId, int? commentId);
}