using Application.DTOs;
using Application.DTOs.Reports;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        IReportRepository reportRepository,
        IPostRepository postRepository,
        ICommentRepository commentRepository,
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<ReportService> logger)
    {
        _reportRepository = reportRepository;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ReportDto> CreateReportAsync(int reporterId, CreateReportDto createReportDto)
    {
        try
        {
            // Validate that either PostId or CommentId is provided
            if (!createReportDto.PostId.HasValue && !createReportDto.CommentId.HasValue)
                throw new InvalidOperationException("Either PostId or CommentId must be provided");

            if (createReportDto.PostId.HasValue && createReportDto.CommentId.HasValue)
                throw new InvalidOperationException("Cannot report both Post and Comment in the same report");

            // Check if user has already reported this content
            var alreadyReported = await _reportRepository.HasUserReportedContentAsync(
                reporterId, createReportDto.PostId, createReportDto.CommentId);

            if (alreadyReported)
                throw new InvalidOperationException("You have already reported this content");

            // Validate the content exists
            if (createReportDto.PostId.HasValue)
            {
                var post = await _postRepository.GetByIdAsync(createReportDto.PostId.Value);
                if (post == null)
                    throw new InvalidOperationException("Post not found");

                // Check if user is trying to report their own post
                if (post.UserId == reporterId)
                    throw new InvalidOperationException("You cannot report your own post");
            }

            if (createReportDto.CommentId.HasValue)
            {
                var comment = await _commentRepository.GetByIdAsync(createReportDto.CommentId.Value);
                if (comment == null)
                    throw new InvalidOperationException("Comment not found");

                // Check if user is trying to report their own comment
                if (comment.UserId == reporterId)
                    throw new InvalidOperationException("You cannot report your own comment");
            }

            var report = new Report
            {
                ReporterId = reporterId,
                Type = createReportDto.Type,
                PostId = createReportDto.PostId,
                CommentId = createReportDto.CommentId,
                Reason = createReportDto.Reason.Trim(),
                AdditionalDetails = createReportDto.AdditionalDetails?.Trim(),
                Status = ReportStatus.Pending
            };

            var createdReport = await _reportRepository.AddAsync(report);
            
            _logger.LogInformation("Report created: {ReportId} by user {ReporterId} for {Type} {ContentId}", 
                createdReport.Id, reporterId, createReportDto.Type,
                createReportDto.PostId ?? createReportDto.CommentId);

            return await MapToReportDtoAsync(createdReport);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating report by user {ReporterId}", reporterId);
            throw;
        }
    }

    public async Task<ReportListDto> GetUserReportsAsync(int userId, int page = 1, int pageSize = 10)
    {
        try
        {
            var (reports, totalCount) = await _reportRepository.GetUserReportsPagedAsync(userId, page, pageSize);
            
            var reportDtos = new List<ReportDto>();
            foreach (var report in reports)
            {
                var dto = await MapToReportDtoAsync(report);
                reportDtos.Add(dto);
            }

            return new ReportListDto
            {
                Reports = reportDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                HasNextPage = (page * pageSize) < totalCount,
                HasPreviousPage = page > 1
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reports for user {UserId}", userId);
            throw;
        }
    }

    public async Task<ReportListDto> GetAllReportsAsync(ReportStatus? status = null, ReportType? type = null, int page = 1, int pageSize = 10)
    {
        try
        {
            var (reports, totalCount) = await _reportRepository.GetPagedReportsAsync(status, type, page, pageSize);
            
            var reportDtos = new List<ReportDto>();
            foreach (var report in reports)
            {
                var dto = await MapToReportDtoAsync(report);
                reportDtos.Add(dto);
            }

            return new ReportListDto
            {
                Reports = reportDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                HasNextPage = (page * pageSize) < totalCount,
                HasPreviousPage = page > 1
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all reports");
            throw;
        }
    }

    public async Task<ReportDto?> GetReportByIdAsync(int reportId)
    {
        try
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null)
                return null;

            return await MapToReportDtoAsync(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting report {ReportId}", reportId);
            throw;
        }
    }

    public async Task<ReportDto> ResolveReportAsync(int reportId, int adminUserId, ResolveReportDto resolveReportDto)
    {
        try
        {
            // Check if admin
            var admin = await _userRepository.GetByIdAsync(adminUserId);
            if (admin?.Role != UserRole.Administrator)
                throw new UnauthorizedAccessException("Only administrators can resolve reports");

            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null)
                throw new InvalidOperationException("Report not found");

            if (report.Status == ReportStatus.Resolved || report.Status == ReportStatus.Dismissed)
                throw new InvalidOperationException("Report has already been resolved");

            // Update report
            report.Status = resolveReportDto.Status;
            report.ResolvedByUserId = adminUserId;
            report.ResolutionNote = resolveReportDto.ResolutionNote.Trim();
            report.ResolvedAt = DateTime.UtcNow;

            await _reportRepository.UpdateAsync(report);
            
            _logger.LogInformation("Report {ReportId} resolved by admin {AdminUserId} with status {Status}", 
                reportId, adminUserId, resolveReportDto.Status);

            return await MapToReportDtoAsync(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving report {ReportId} by admin {AdminUserId}", reportId, adminUserId);
            throw;
        }
    }

    public async Task<Dictionary<ReportStatus, int>> GetReportStatisticsAsync()
    {
        try
        {
            return await _reportRepository.GetReportStatisticsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting report statistics");
            throw;
        }
    }

    public async Task<Dictionary<ReportType, int>> GetReportTypeStatisticsAsync()
    {
        try
        {
            return await _reportRepository.GetReportTypeStatisticsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting report type statistics");
            throw;
        }
    }

    public async Task<bool> IsContentReportedAsync(int? postId, int? commentId)
    {
        try
        {
            var reportCount = await _reportRepository.GetContentReportCountAsync(postId, commentId);
            return reportCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if content is reported");
            throw;
        }
    }

    public async Task<int> GetContentReportCountAsync(int? postId, int? commentId)
    {
        try
        {
            return await _reportRepository.GetContentReportCountAsync(postId, commentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting content report count");
            throw;
        }
    }

    private async Task<ReportDto> MapToReportDtoAsync(Report report)
    {
        var reporterDto = _mapper.Map<UserDto>(report.Reporter);
        var resolvedByUserDto = report.ResolvedByUser != null ? _mapper.Map<UserDto>(report.ResolvedByUser) : null;

        string? reportedContent = null;
        string? reportedUserName = null;

        if (report.Post != null)
        {
            reportedContent = report.Post.Content;
            reportedUserName = report.Post.User?.Name;
        }
        else if (report.Comment != null)
        {
            reportedContent = report.Comment.Content;
            reportedUserName = report.Comment.User?.Name;
        }

        return new ReportDto
        {
            Id = report.Id,
            ReporterId = report.ReporterId,
            Type = report.Type,
            PostId = report.PostId,
            CommentId = report.CommentId,
            Reason = report.Reason,
            AdditionalDetails = report.AdditionalDetails,
            Status = report.Status,
            ResolvedByUserId = report.ResolvedByUserId,
            ResolutionNote = report.ResolutionNote,
            CreatedAt = report.CreatedAt,
            ResolvedAt = report.ResolvedAt,
            Reporter = reporterDto,
            ResolvedByUser = resolvedByUserDto,
            ReportedContent = reportedContent,
            ReportedUserName = reportedUserName
        };
    }
}