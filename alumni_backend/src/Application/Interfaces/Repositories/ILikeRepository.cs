using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ILikeRepository : IBaseRepository<Like>
{
    // Post likes
    Task<Like?> GetLikeAsync(int userId, int postId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Like>> GetByPostIdAsync(int postId, CancellationToken cancellationToken = default);
    Task<Like?> GetByUserAndPostAsync(int userId, int postId, CancellationToken cancellationToken = default);
    Task<bool> HasUserLikedPostAsync(int userId, int postId, CancellationToken cancellationToken = default);
    Task<int> GetLikeCountAsync(int postId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Like>> GetUserLikesAsync(int userId, CancellationToken cancellationToken = default);
    
    // Comment likes (Phase 2)
    Task<Like?> GetCommentLikeAsync(int userId, int commentId, CancellationToken cancellationToken = default);
    Task<bool> HasUserLikedCommentAsync(int userId, int commentId, CancellationToken cancellationToken = default);
    Task<int> GetCommentLikeCountAsync(int commentId, CancellationToken cancellationToken = default);
}