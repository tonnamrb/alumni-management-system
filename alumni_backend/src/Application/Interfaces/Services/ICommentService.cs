using Application.DTOs.Comments;

namespace Application.Interfaces.Services;

public interface ICommentService
{
    // Comments CRUD
    Task<CommentListDto> GetCommentsAsync(int postId, int page = 1, int pageSize = 10, int? currentUserId = null);
    Task<CommentDto?> GetCommentByIdAsync(int commentId, int? currentUserId = null);
    Task<CommentDto> CreateCommentAsync(int userId, CreateCommentDto createCommentDto);
    Task<CommentDto> UpdateCommentAsync(int commentId, int userId, UpdateCommentDto updateCommentDto);
    Task<bool> DeleteCommentAsync(int commentId, int userId, bool isAdmin = false);
    
    // Comment Interactions
    Task<CommentDto> ToggleCommentLikeAsync(int commentId, int userId);
    Task<List<CommentDto>> GetRepliesAsync(int parentCommentId, int? currentUserId = null);
    
    // Admin features
    Task<bool> DeleteCommentByAdminAsync(int commentId, int adminUserId);
    Task<CommentListDto> GetReportedCommentsAsync(int page = 1, int pageSize = 10);
}