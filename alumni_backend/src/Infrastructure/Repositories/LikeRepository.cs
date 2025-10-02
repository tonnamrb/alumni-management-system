using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LikeRepository : BaseRepository<Like>, ILikeRepository
{
    public LikeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Like?> GetLikeAsync(int userId, int postId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId, cancellationToken);
    }

    public async Task<IEnumerable<Like>> GetByPostIdAsync(int postId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(l => l.User)
            .Where(l => l.PostId == postId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Like?> GetByUserAndPostAsync(int userId, int postId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId, cancellationToken);
    }

    public async Task<bool> HasUserLikedPostAsync(int userId, int postId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(l => l.UserId == userId && l.PostId == postId, cancellationToken);
    }

    public async Task<int> GetLikeCountAsync(int postId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(l => l.PostId == postId, cancellationToken);
    }

    public async Task<IEnumerable<Like>> GetUserLikesAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(l => l.Post)
                .ThenInclude(p => p.User)
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    // Enhanced methods for social features  
    public async Task<Like?> GetLikeAsync(int postId, int userId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);
    }

    public async Task<List<Like>> GetPostLikesAsync(int postId)
    {
        return await _dbSet
            .Include(l => l.User)
            .Where(l => l.PostId == postId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetPostLikesCountAsync(int postId)
    {
        return await _dbSet
            .Where(l => l.PostId == postId)
            .CountAsync();
    }

    public async Task<Like?> GetCommentLikeAsync(int userId, int commentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(l => l.UserId == userId && l.CommentId == commentId, cancellationToken);
    }

    public async Task<bool> HasUserLikedCommentAsync(int userId, int commentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(l => l.UserId == userId && l.CommentId == commentId, cancellationToken);
    }

    public async Task<int> GetCommentLikeCountAsync(int commentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(l => l.CommentId == commentId, cancellationToken);
    }
}