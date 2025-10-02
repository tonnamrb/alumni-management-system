using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IAlumniProfileRepository : IBaseRepository<AlumniProfile>
{
    // Existing methods
    Task<AlumniProfile?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AlumniProfile>> GetPublicProfilesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AlumniProfile>> SearchByMajorAsync(string major, CancellationToken cancellationToken = default);
    Task<IEnumerable<AlumniProfile>> SearchByGraduationYearAsync(string graduationYear, CancellationToken cancellationToken = default);
    Task<IEnumerable<AlumniProfile>> SearchByCompanyAsync(string company, CancellationToken cancellationToken = default);
    Task<AlumniProfile?> GetWithUserAsync(int profileId, CancellationToken cancellationToken = default);
    
    // External data integration methods
    Task<AlumniProfile?> GetByExternalMemberIDAsync(string externalMemberID, CancellationToken cancellationToken = default);
    Task<List<AlumniProfile>> GetUnverifiedProfilesAsync(CancellationToken cancellationToken = default);
    Task<List<AlumniProfile>> GetByExternalSystemIdAsync(string systemId, CancellationToken cancellationToken = default);
    Task<List<AlumniProfile>> GetProfilesNeedingSyncAsync(string? systemId = null, CancellationToken cancellationToken = default);
    
    // Data management methods
    Task<PagedResult<AlumniProfile>> GetAllPaginatedAsync(int page, int pageSize, string? search = null, int? graduationYear = null, string? major = null, string? externalSystemId = null, CancellationToken cancellationToken = default);
}