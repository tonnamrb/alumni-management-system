using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface IPostRepository : IBaseRepository<Post>
{
    // Existing methods
    Task<IEnumerable<Post>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetPinnedPostsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetPinnedPostsAsync(PostType? type, CancellationToken cancellationToken = default);
    Task<Post?> GetWithCommentsAsync(int postId, CancellationToken cancellationToken = default);
    Task<Post?> GetWithLikesAsync(int postId, CancellationToken cancellationToken = default);
    Task<Post?> GetWithUserAsync(int postId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetRecentPostsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> SearchPostsAsync(string searchTerm, CancellationToken cancellationToken = default);
    
    // Enhanced methods for social features
    Task<Post?> GetByIdWithUserAsync(int id);
    Task<List<Post>> GetPostsWithUserAndLikesAsync(int page, int pageSize);
    Task<List<Post>> GetPostsWithUserAndLikesAsync(int page, int pageSize, PostType? type);
    Task<int> GetPostsCountAsync();
    Task<int> GetPostsCountAsync(PostType? type);
    Task<int> GetPinnedPostsCountAsync();
    Task<bool> IsLikedByUserAsync(int postId, int userId);
    Task<int> GetLikesCountAsync(int postId);
}