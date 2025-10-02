using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AlumniProfileRepository : BaseRepository<AlumniProfile>, IAlumniProfileRepository
{
    public AlumniProfileRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<AlumniProfile?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<AlumniProfile>> GetPublicProfilesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.User)
            .Where(p => p.IsProfilePublic && p.User.IsActive)
            .OrderBy(p => p.User.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AlumniProfile>> SearchByMajorAsync(string major, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.User)
            .Where(p => p.IsProfilePublic && 
                       p.User.IsActive && 
                       p.Major != null && 
                       p.Major.ToLower().Contains(major.ToLower()))
            .OrderBy(p => p.User.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AlumniProfile>> SearchByGraduationYearAsync(string graduationYear, CancellationToken cancellationToken = default)
    {
        // Convert string to int for comparison
        if (int.TryParse(graduationYear, out int year))
        {
            return await _dbSet
                .Include(p => p.User)
                .Where(p => p.IsProfilePublic && 
                           p.User.IsActive && 
                           p.GraduationYear == year)
                .OrderBy(p => p.User.Name)
                .ToListAsync(cancellationToken);
        }
        
        return new List<AlumniProfile>();
    }

    public async Task<IEnumerable<AlumniProfile>> SearchByCompanyAsync(string company, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.User)
            .Where(p => p.IsProfilePublic && 
                       p.User.IsActive && 
                       p.CurrentCompany != null && 
                       p.CurrentCompany.ToLower().Contains(company.ToLower()))
            .OrderBy(p => p.User.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<AlumniProfile?> GetWithUserAsync(int profileId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == profileId, cancellationToken);
    }

    // External data integration methods
    public async Task<AlumniProfile?> GetByExternalMemberIDAsync(string externalMemberID, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.ExternalMemberID == externalMemberID, cancellationToken);
    }

    public async Task<List<AlumniProfile>> GetUnverifiedProfilesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.User)
            .Where(p => !p.IsVerified && p.ExternalSystemId != null)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AlumniProfile>> GetByExternalSystemIdAsync(string systemId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.User)
            .Where(p => p.ExternalSystemId == systemId)
            .OrderBy(p => p.User.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AlumniProfile>> GetProfilesNeedingSyncAsync(string? systemId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.User)
            .Where(p => p.ExternalSystemId != null);

        if (!string.IsNullOrWhiteSpace(systemId))
        {
            query = query.Where(p => p.ExternalSystemId == systemId);
        }

        // Profiles that haven't been synced in the last 24 hours or never synced
        var cutoffTime = DateTime.UtcNow.AddHours(-24);
        query = query.Where(p => p.ExternalDataLastSync == null || p.ExternalDataLastSync < cutoffTime);

        return await query
            .OrderBy(p => p.ExternalDataLastSync ?? DateTime.MinValue)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<AlumniProfile>> GetAllPaginatedAsync(
        int page, 
        int pageSize, 
        string? search = null, 
        int? graduationYear = null, 
        string? major = null, 
        string? externalSystemId = null, 
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => 
                (p.NameInYearbook != null && p.NameInYearbook.Contains(search)) ||
                (p.Firstname != null && p.Firstname.Contains(search)) ||
                (p.Lastname != null && p.Lastname.Contains(search)) ||
                (p.MobilePhone != null && p.MobilePhone.Contains(search)) ||
                (p.Email != null && p.Email.Contains(search)) ||
                (p.CompanyName != null && p.CompanyName.Contains(search)) ||
                (p.ExternalMemberID != null && p.ExternalMemberID.Contains(search)));
        }

        // Apply graduation year filter
        if (graduationYear.HasValue)
        {
            query = query.Where(p => p.GraduationYear == graduationYear.Value);
        }

        // Apply major filter
        if (!string.IsNullOrWhiteSpace(major))
        {
            query = query.Where(p => p.Major != null && p.Major.Contains(major));
        }

        // Apply external system filter
        if (!string.IsNullOrWhiteSpace(externalSystemId))
        {
            query = query.Where(p => p.ExternalSystemId == externalSystemId);
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var items = await query
            .OrderBy(p => p.NameInYearbook ?? p.Firstname)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<AlumniProfile>
        {
            Items = items,
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }
}