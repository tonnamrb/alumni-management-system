using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    // Existing methods
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByProviderAsync(string provider, string providerId, CancellationToken cancellationToken = default);
    Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
    Task<User?> GetWithProfileAsync(int userId, CancellationToken cancellationToken = default);
    
    // Mobile phone authentication methods
    Task<User?> GetByMobilePhoneAsync(string mobilePhone, CancellationToken cancellationToken = default);
    Task<bool> IsMobilePhoneExistsAsync(string mobilePhone, int? excludeUserId = null, CancellationToken cancellationToken = default);
    
    // External data integration methods
    Task<User?> GetByExternalMemberIDAsync(string externalMemberID, CancellationToken cancellationToken = default);
    Task<List<User>> GetByExternalSystemIdAsync(string systemId, CancellationToken cancellationToken = default);
    Task<List<User>> GetUsersNeedingSyncAsync(string? systemId = null, CancellationToken cancellationToken = default);
    
    // Data management methods
    Task<PagedResult<User>> GetAllPaginatedAsync(int page, int pageSize, string? search = null, string? externalSystemId = null, CancellationToken cancellationToken = default);
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}