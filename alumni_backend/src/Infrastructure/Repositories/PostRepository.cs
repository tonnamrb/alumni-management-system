using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PostRepository : BaseRepository<Post>, IPostRepository
{
    public PostRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Post>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.User)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetPinnedPostsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.User)
            .Where(p => p.IsPinned && true)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Post?> GetWithCommentsAsync(int postId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.Replies)
                    .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);
    }

    public async Task<Post?> GetWithLikesAsync(int postId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.User)
            .Include(p => p.Likes)
                .ThenInclude(l => l.User)
            .FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);
    }

    public async Task<Post?> GetWithUserAsync(int postId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetRecentPostsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.User)
            .Where(p => true)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> SearchPostsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.User)
            .Where(p => true && 
                       p.Content.ToLower().Contains(searchTerm.ToLower()))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    // Enhanced methods for social features
    public async Task<Post?> GetByIdWithUserAsync(int id)
    {
        return await _dbSet
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Post>> GetPostsWithUserAndLikesAsync(int page, int pageSize)
    {
        return await _dbSet
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .Where(p => true)
            .OrderByDescending(p => p.IsPinned)
            .ThenByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetPostsCountAsync()
    {
        return await _dbSet
            .Include(p => p.User)
            .Where(p => true)
            .CountAsync();
    }

    public async Task<int> GetPinnedPostsCountAsync()
    {
        return await _dbSet
            .Include(p => p.User)
            .Where(p => p.IsPinned && true)
            .CountAsync();
    }

    public async Task<bool> IsLikedByUserAsync(int postId, int userId)
    {
        return await _context.Set<Like>()
            .AnyAsync(l => l.PostId == postId && l.UserId == userId);
    }

    public async Task<int> GetLikesCountAsync(int postId)
    {
        return await _context.Set<Like>()
            .Where(l => l.PostId == postId)
            .CountAsync();
    }

    // Missing methods from interface
    public async Task<IEnumerable<Post>> GetPinnedPostsAsync(PostType? type, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.User)
            .Where(p => p.IsPinned);

        if (type.HasValue)
        {
            query = query.Where(p => p.Type == type.Value);
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Post>> GetPostsWithUserAndLikesAsync(int page, int pageSize, PostType? type)
    {
        var query = _dbSet
            .Include(p => p.User)
            .Include(p => p.Likes)
            .AsQueryable();

        if (type.HasValue)
        {
            query = query.Where(p => p.Type == type.Value);
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetPostsCountAsync(PostType? type)
    {
        var query = _dbSet.AsQueryable();
        
        if (type.HasValue)
        {
            query = query.Where(p => p.Type == type.Value);
        }

        return await query.CountAsync();
    }
}
