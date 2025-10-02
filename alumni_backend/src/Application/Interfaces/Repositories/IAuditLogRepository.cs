using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface IAuditLogRepository : IBaseRepository<AuditLog>
{
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByActionAsync(AuditAction action, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, int entityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetRecentActivitiesAsync(int count, CancellationToken cancellationToken = default);
}