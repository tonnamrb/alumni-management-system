using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CommentRepository : BaseRepository<Comment>, ICommentRepository
{
    public CommentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Comment>> GetByPostIdAsync(int postId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.User)
            .Where(c => c.PostId == postId && c.ParentCommentId == null)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Comment>> GetRepliesAsync(int parentCommentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.User)
            .Where(c => c.ParentCommentId == parentCommentId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Comment?> GetWithRepliesAsync(int commentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.Replies)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(c => c.Id == commentId, cancellationToken);
    }

    public async Task<Comment?> GetWithUserAsync(int commentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == commentId, cancellationToken);
    }

    public async Task<IEnumerable<Comment>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Post)
            .Include(c => c.User)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetReplyCountAsync(int commentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(c => c.ParentCommentId == commentId, cancellationToken);
    }

    public async Task<(IEnumerable<Comment> Comments, int TotalCount)> GetPagedByPostIdAsync(
        int postId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(c => c.User)
            .Include(c => c.Likes)
            .Where(c => c.PostId == postId && c.ParentCommentId == null) // Only top-level comments
            .OrderByDescending(c => c.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var comments = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (comments, totalCount);
    }



    public async Task<IEnumerable<Comment>> GetReportedCommentsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.Post)
            .Where(c => c.Reports.Any()) // Comments that have reports
            .OrderByDescending(c => c.Reports.Count())
            .ThenByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}