using Application.DTOs.Comments;
using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILikeRepository _likeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CommentService> _logger;

    public CommentService(
        ICommentRepository commentRepository,
        IPostRepository postRepository,
        IUserRepository userRepository,
        ILikeRepository likeRepository,
        IMapper mapper,
        ILogger<CommentService> logger)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _userRepository = userRepository;
        _likeRepository = likeRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CommentListDto> GetCommentsAsync(int postId, int page = 1, int pageSize = 10, int? currentUserId = null)
    {
        try
        {
            var (comments, totalCount) = await _commentRepository.GetPagedByPostIdAsync(postId, page, pageSize);
            
            var commentDtos = new List<CommentDto>();
            
            foreach (var comment in comments)
            {
                var dto = await MapToCommentDtoAsync(comment, currentUserId);
                commentDtos.Add(dto);
            }
            
            return new CommentListDto
            {
                Comments = commentDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                HasNextPage = (page * pageSize) < totalCount,
                HasPreviousPage = page > 1
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comments for post {PostId}", postId);
            throw;
        }
    }

    public async Task<CommentDto?> GetCommentByIdAsync(int commentId, int? currentUserId = null)
    {
        try
        {
            var comment = await _commentRepository.GetWithUserAsync(commentId);
            if (comment == null)
                return null;

            return await MapToCommentDtoAsync(comment, currentUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comment {CommentId}", commentId);
            throw;
        }
    }

    public async Task<CommentDto> CreateCommentAsync(int userId, CreateCommentDto createCommentDto)
    {
        try
        {
            // Validate post exists
            var post = await _postRepository.GetByIdAsync(createCommentDto.PostId);
            if (post == null)
                throw new InvalidOperationException("Post not found");

            // Validate parent comment if specified
            if (createCommentDto.ParentCommentId.HasValue)
            {
                var parentComment = await _commentRepository.GetByIdAsync(createCommentDto.ParentCommentId.Value);
                if (parentComment == null || parentComment.PostId != createCommentDto.PostId)
                    throw new InvalidOperationException("Parent comment not found or doesn't belong to the same post");
            }

            var comment = new Comment
            {
                UserId = userId,
                PostId = createCommentDto.PostId,
                ParentCommentId = createCommentDto.ParentCommentId,
                Content = createCommentDto.Content.Trim(),
                MentionedUserIds = createCommentDto.MentionedUserIds != null ? 
                    string.Join(",", createCommentDto.MentionedUserIds) : null
            };

            var createdComment = await _commentRepository.AddAsync(comment);
            
            _logger.LogInformation("Comment created: {CommentId} by user {UserId}", createdComment.Id, userId);
            
            return await MapToCommentDtoAsync(createdComment, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating comment for user {UserId}", userId);
            throw;
        }
    }

    public async Task<CommentDto> UpdateCommentAsync(int commentId, int userId, UpdateCommentDto updateCommentDto)
    {
        try
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
                throw new InvalidOperationException("Comment not found");

            if (comment.UserId != userId)
                throw new UnauthorizedAccessException("Only the comment owner can update this comment");

            comment.UpdateContent(updateCommentDto.Content.Trim());
            
            if (updateCommentDto.MentionedUserIds != null)
            {
                comment.MentionedUserIds = string.Join(",", updateCommentDto.MentionedUserIds);
            }

            await _commentRepository.UpdateAsync(comment);
            
            _logger.LogInformation("Comment updated: {CommentId} by user {UserId}", commentId, userId);
            
            return await MapToCommentDtoAsync(comment, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating comment {CommentId} by user {UserId}", commentId, userId);
            throw;
        }
    }

    public async Task<bool> DeleteCommentAsync(int commentId, int userId, bool isAdmin = false)
    {
        try
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
                return false;

            if (!isAdmin && comment.UserId != userId)
                throw new UnauthorizedAccessException("Only the comment owner or admin can delete this comment");

            await _commentRepository.DeleteAsync(comment);
            
            _logger.LogInformation("Comment deleted: {CommentId} by user {UserId} (Admin: {IsAdmin})", commentId, userId, isAdmin);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment {CommentId} by user {UserId}", commentId, userId);
            throw;
        }
    }

    public async Task<CommentDto> ToggleCommentLikeAsync(int commentId, int userId)
    {
        try
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
                throw new InvalidOperationException("Comment not found");

            var existingLike = await _likeRepository.GetCommentLikeAsync(userId, commentId);
            
            if (existingLike != null)
            {
                // Unlike
                await _likeRepository.DeleteAsync(existingLike);
                _logger.LogInformation("Comment unliked: {CommentId} by user {UserId}", commentId, userId);
            }
            else
            {
                // Like
                var like = Like.CreateForComment(userId, commentId);
                await _likeRepository.AddAsync(like);
                _logger.LogInformation("Comment liked: {CommentId} by user {UserId}", commentId, userId);
            }

            return await MapToCommentDtoAsync(comment, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling like for comment {CommentId} by user {UserId}", commentId, userId);
            throw;
        }
    }

    public async Task<List<CommentDto>> GetRepliesAsync(int parentCommentId, int? currentUserId = null)
    {
        try
        {
            var replies = await _commentRepository.GetRepliesAsync(parentCommentId);
            
            var replyDtos = new List<CommentDto>();
            
            foreach (var reply in replies)
            {
                var dto = await MapToCommentDtoAsync(reply, currentUserId);
                replyDtos.Add(dto);
            }
            
            return replyDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting replies for comment {ParentCommentId}", parentCommentId);
            throw;
        }
    }

    public async Task<bool> DeleteCommentByAdminAsync(int commentId, int adminUserId)
    {
        try
        {
            // Check if admin
            var admin = await _userRepository.GetByIdAsync(adminUserId);
            if (admin?.Role != UserRole.Administrator)
                throw new UnauthorizedAccessException("Only admins can delete comments");

            return await DeleteCommentAsync(commentId, adminUserId, isAdmin: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment {CommentId} by admin {AdminUserId}", commentId, adminUserId);
            throw;
        }
    }

    public async Task<CommentListDto> GetReportedCommentsAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            var reportedComments = await _commentRepository.GetReportedCommentsAsync(page, pageSize);
            
            var commentDtos = new List<CommentDto>();
            
            foreach (var comment in reportedComments)
            {
                var dto = await MapToCommentDtoAsync(comment, null);
                commentDtos.Add(dto);
            }
            
            // Get total count (simplified for now)
            var totalCount = commentDtos.Count;
            
            return new CommentListDto
            {
                Comments = commentDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                HasNextPage = commentDtos.Count == pageSize,
                HasPreviousPage = page > 1
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reported comments");
            throw;
        }
    }

    private async Task<CommentDto> MapToCommentDtoAsync(Comment comment, int? currentUserId)
    {
        var userDto = _mapper.Map<UserDto>(comment.User);
        
        var isLiked = currentUserId.HasValue ? 
            await _likeRepository.HasUserLikedCommentAsync(currentUserId.Value, comment.Id) : false;
            
        var likeCount = await _likeRepository.GetCommentLikeCountAsync(comment.Id);
        
        // Get replies
        var replies = await _commentRepository.GetRepliesAsync(comment.Id);
        var replyDtos = new List<CommentDto>();
        
        foreach (var reply in replies)
        {
            var replyDto = await MapToCommentDtoAsync(reply, currentUserId);
            replyDtos.Add(replyDto);
        }

        // Parse mentioned user IDs
        var mentionedUserIds = string.IsNullOrEmpty(comment.MentionedUserIds) 
            ? null 
            : comment.MentionedUserIds.Split(',').Select(int.Parse).ToList();

        return new CommentDto
        {
            Id = comment.Id,
            UserId = comment.UserId,
            PostId = comment.PostId,
            ParentCommentId = comment.ParentCommentId,
            Content = comment.Content,
            MentionedUserIds = mentionedUserIds,
            LikesCount = likeCount,
            IsLikedByCurrentUser = isLiked,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            User = userDto,
            Replies = replyDtos.Any() ? replyDtos : null
        };
    }
}