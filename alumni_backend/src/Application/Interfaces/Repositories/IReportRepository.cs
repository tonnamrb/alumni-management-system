using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface IReportRepository : IBaseRepository<Report>
{
    Task<IEnumerable<Report>> GetByReporterIdAsync(int reporterId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Report>> GetByStatusAsync(ReportStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Report>> GetByTypeAsync(ReportType type, CancellationToken cancellationToken = default);
    Task<Report?> GetByUserAndEntityAsync(int reporterId, string entityType, int entityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Report>> GetPendingReportsAsync(CancellationToken cancellationToken = default);
    Task<int> GetReportCountForEntityAsync(string entityType, int entityId, CancellationToken cancellationToken = default);
    
    // Phase 3 additions
    Task<(IEnumerable<Report> Reports, int TotalCount)> GetPagedReportsAsync(
        ReportStatus? status, ReportType? type, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Report> Reports, int TotalCount)> GetUserReportsPagedAsync(
        int userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Dictionary<ReportStatus, int>> GetReportStatisticsAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<ReportType, int>> GetReportTypeStatisticsAsync(CancellationToken cancellationToken = default);
    Task<bool> HasUserReportedContentAsync(int userId, int? postId, int? commentId, CancellationToken cancellationToken = default);
    Task<int> GetContentReportCountAsync(int? postId, int? commentId, CancellationToken cancellationToken = default);
}