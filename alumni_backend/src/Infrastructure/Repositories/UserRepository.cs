using Application.Helpers;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(AppDbContext context, ILogger<UserRepository> logger) : base(context)
    {
        _logger = logger;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<User?> GetByProviderAsync(string provider, string providerId, CancellationToken cancellationToken = default)
    {
        // This method is kept for compatibility but not used in the new schema
        // Since we removed Provider/ProviderId fields, we'll search by email as fallback
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == providerId, cancellationToken);
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
            .OrderBy(u => u.Firstname)
            .ThenBy(u => u.Lastname)
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
        // Get last 9 digits for comparison (handles mixed formats in DB: +66xxx, 66xxx, 08xxx, 09xxx)
        var searchLast9Digits = PhoneNumberHelper.GetLast9Digits(mobilePhone);
        
        _logger.LogInformation($"UserRepository.GetByMobilePhoneAsync: Input phone: {mobilePhone}, Search last 9 digits: {searchLast9Digits}");
        
        var users = await _dbSet
            .Include(u => u.AlumniProfile)
            .Where(u => !string.IsNullOrEmpty(u.MobilePhone))
            .ToListAsync(cancellationToken);
            
        _logger.LogInformation($"UserRepository.GetByMobilePhoneAsync: Found {users.Count} users with non-empty mobile phones");
        
        foreach (var user in users)
        {
            var userLast9 = PhoneNumberHelper.GetLast9Digits(user.MobilePhone!);
            _logger.LogInformation($"UserRepository.GetByMobilePhoneAsync: User ID: {user.Id}, Phone: {user.MobilePhone}, Last 9 digits: {userLast9}, Match: {userLast9 == searchLast9Digits}");
        }
            
        return users
            .Where(u => u.MobilePhone != null && PhoneNumberHelper.GetLast9Digits(u.MobilePhone) == searchLast9Digits)
            .FirstOrDefault();
    }

    public async Task<bool> IsMobilePhoneExistsAsync(string mobilePhone, int? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        // Get last 9 digits for comparison (handles mixed formats in DB: +66xxx, 66xxx, 08xxx, 09xxx)
        var searchLast9Digits = PhoneNumberHelper.GetLast9Digits(mobilePhone);
        
        var query = _dbSet.Where(u => !string.IsNullOrEmpty(u.MobilePhone));
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }
        
        var users = await query.ToListAsync(cancellationToken);
        
        return users.Any(u => u.MobilePhone != null && PhoneNumberHelper.GetLast9Digits(u.MobilePhone) == searchLast9Digits);
    }

    // External data integration methods (updated for new schema)
    public async Task<User?> GetByMemberIDAsync(string memberID, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.AlumniProfile)
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.MemberID == memberID, cancellationToken);
    }

    public async Task<List<User>> GetUsersByGroupIDAsync(string groupID, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.AlumniProfile)
            .Include(u => u.Role)
            .Where(u => u.GroupID == groupID && u.RoleId == 1) // Only alumni members
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetAlumniMembersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.AlumniProfile)
            .Include(u => u.Role)
            .Where(u => u.RoleId == 1) // Only alumni members
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
            search = search.ToLower();
            query = query.Where(u => 
                u.Firstname.ToLower().Contains(search) ||
                u.Lastname.ToLower().Contains(search) ||
                (u.Email != null && u.Email.ToLower().Contains(search)) ||
                (u.MobilePhone != null && u.MobilePhone.Contains(search)) ||
                (u.MemberID != null && u.MemberID.ToLower().Contains(search)));
        }

        // External system filter is not supported in new schema - ignore

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var items = await query
            .OrderBy(u => u.Firstname)
            .ThenBy(u => u.Lastname)
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