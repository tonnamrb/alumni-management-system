using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ICommentRepository : IBaseRepository<Comment>
{
    Task<IEnumerable<Comment>> GetByPostIdAsync(int postId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Comment>> GetRepliesAsync(int parentCommentId, CancellationToken cancellationToken = default);
    Task<Comment?> GetWithRepliesAsync(int commentId, CancellationToken cancellationToken = default);
    Task<Comment?> GetWithUserAsync(int commentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Comment>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<int> GetReplyCountAsync(int commentId, CancellationToken cancellationToken = default);
    
    // Phase 2 additions
    Task<(IEnumerable<Comment> Comments, int TotalCount)> GetPagedByPostIdAsync(
        int postId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Comment>> GetReportedCommentsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}