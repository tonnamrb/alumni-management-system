using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<User?> GetByProviderAsync(string provider, string providerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Provider == provider && u.ProviderId == providerId, cancellationToken);
    }

    public async Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(u => u.Email.ToLower() == email.ToLower());
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }
        
        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u => u.IsActive)
            .OrderBy(u => u.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetWithProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.AlumniProfile)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    // Mobile phone authentication methods
    public async Task<User?> GetByMobilePhoneAsync(string mobilePhone, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.AlumniProfile)
            .FirstOrDefaultAsync(u => u.MobilePhone == mobilePhone, cancellationToken);
    }

    public async Task<bool> IsMobilePhoneExistsAsync(string mobilePhone, int? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(u => u.MobilePhone == mobilePhone);
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }
        
        return await query.AnyAsync(cancellationToken);
    }

    // External data integration methods
    public async Task<User?> GetByExternalMemberIDAsync(string externalMemberID, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.AlumniProfile)
            .FirstOrDefaultAsync(u => u.ExternalMemberID == externalMemberID, cancellationToken);
    }

    public async Task<List<User>> GetByExternalSystemIdAsync(string systemId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.AlumniProfile)
            .Where(u => u.ExternalSystemId == systemId)
            .OrderBy(u => u.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetUsersNeedingSyncAsync(string? systemId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(u => u.AlumniProfile)
            .Where(u => u.ExternalSystemId != null);

        if (!string.IsNullOrWhiteSpace(systemId))
        {
            query = query.Where(u => u.ExternalSystemId == systemId);
        }

        // Users that haven't been synced in the last 24 hours or never synced
        var cutoffTime = DateTime.UtcNow.AddHours(-24);
        query = query.Where(u => u.ExternalDataLastSync == null || u.ExternalDataLastSync < cutoffTime);

        return await query
            .OrderBy(u => u.ExternalDataLastSync ?? DateTime.MinValue)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<User>> GetAllPaginatedAsync(
        int page, 
        int pageSize, 
        string? search = null, 
        string? externalSystemId = null, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users.AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u => 
                u.Name.Contains(search) ||
                (u.Email != null && u.Email.Contains(search)) ||
                u.MobilePhone.Contains(search) ||
                (u.ExternalMemberID != null && u.ExternalMemberID.Contains(search)));
        }

        // Apply external system filter
        if (!string.IsNullOrWhiteSpace(externalSystemId))
        {
            query = query.Where(u => u.ExternalSystemId == externalSystemId);
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var items = await query
            .OrderBy(u => u.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<User>
        {
            Items = items,
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }
}