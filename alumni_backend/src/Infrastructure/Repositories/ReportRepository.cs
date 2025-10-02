using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ReportRepository : BaseRepository<Report>, IReportRepository
{
    public ReportRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Report>> GetByReporterIdAsync(int reporterId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Reporter)
            .Where(r => r.ReporterId == reporterId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Report>> GetByStatusAsync(ReportStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Reporter)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Report>> GetByTypeAsync(ReportType type, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Reporter)
            .Where(r => r.Type == type)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Report?> GetByUserAndEntityAsync(int reporterId, string entityType, int entityId, CancellationToken cancellationToken = default)
    {
        if (entityType.ToLower() == "post")
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.ReporterId == reporterId && r.PostId == entityId, 
                                    cancellationToken);
        }
        else if (entityType.ToLower() == "comment")
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.ReporterId == reporterId && r.CommentId == entityId, 
                                    cancellationToken);
        }
        
        return null;
    }

    public async Task<IEnumerable<Report>> GetPendingReportsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Reporter)
            .Where(r => r.Status == ReportStatus.Pending)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetReportCountForEntityAsync(string entityType, int entityId, CancellationToken cancellationToken = default)
    {
        if (entityType.ToLower() == "post")
        {
            return await _dbSet.CountAsync(r => r.PostId == entityId, cancellationToken);
        }
        else if (entityType.ToLower() == "comment")
        {
            return await _dbSet.CountAsync(r => r.CommentId == entityId, cancellationToken);
        }
        
        return 0;
    }

    public async Task<(IEnumerable<Report> Reports, int TotalCount)> GetPagedReportsAsync(
        ReportStatus? status, ReportType? type, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(r => r.Reporter)
            .Include(r => r.ResolvedByUser)
            .Include(r => r.Post)
                .ThenInclude(p => p.User)
            .Include(r => r.Comment)
                .ThenInclude(c => c.User)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        if (type.HasValue)
            query = query.Where(r => r.Type == type.Value);

        query = query.OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var reports = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (reports, totalCount);
    }

    public async Task<(IEnumerable<Report> Reports, int TotalCount)> GetUserReportsPagedAsync(
        int userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(r => r.Post)
                .ThenInclude(p => p.User)
            .Include(r => r.Comment)
                .ThenInclude(c => c.User)
            .Include(r => r.ResolvedByUser)
            .Where(r => r.ReporterId == userId)
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var reports = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (reports, totalCount);
    }

    public async Task<Dictionary<ReportStatus, int>> GetReportStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var stats = await _dbSet
            .GroupBy(r => r.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return stats.ToDictionary(s => s.Status, s => s.Count);
    }

    public async Task<Dictionary<ReportType, int>> GetReportTypeStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var stats = await _dbSet
            .GroupBy(r => r.Type)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return stats.ToDictionary(s => s.Type, s => s.Count);
    }

    public async Task<bool> HasUserReportedContentAsync(int userId, int? postId, int? commentId, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(r => r.ReporterId == userId);

        if (postId.HasValue)
            query = query.Where(r => r.PostId == postId.Value);
        
        if (commentId.HasValue)
            query = query.Where(r => r.CommentId == commentId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<int> GetContentReportCountAsync(int? postId, int? commentId, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (postId.HasValue)
            query = query.Where(r => r.PostId == postId.Value);
        
        if (commentId.HasValue)
            query = query.Where(r => r.CommentId == commentId.Value);

        return await query.CountAsync(cancellationToken);
    }
}