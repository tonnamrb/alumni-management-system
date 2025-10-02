using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IPostRepository : IBaseRepository<Post>
{
    // Existing methods
    Task<IEnumerable<Post>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetPinnedPostsAsync(CancellationToken cancellationToken = default);
    Task<Post?> GetWithCommentsAsync(int postId, CancellationToken cancellationToken = default);
    Task<Post?> GetWithLikesAsync(int postId, CancellationToken cancellationToken = default);
    Task<Post?> GetWithUserAsync(int postId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetRecentPostsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> SearchPostsAsync(string searchTerm, CancellationToken cancellationToken = default);
    
    // Enhanced methods for social features
    Task<Post?> GetByIdWithUserAsync(int id);
    Task<List<Post>> GetPostsWithUserAndLikesAsync(int page, int pageSize);
    Task<int> GetPostsCountAsync();
    Task<int> GetPinnedPostsCountAsync();
    Task<bool> IsLikedByUserAsync(int postId, int userId);
    Task<int> GetLikesCountAsync(int postId);
}