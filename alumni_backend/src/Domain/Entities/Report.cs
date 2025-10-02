using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Content moderation system linking reported posts/comments to reporting users and resolution status
/// Supports FR-019 to FR-023: Content Management & Moderation
/// </summary>
public class Report : BaseEntity
{
    public int ReporterId { get; set; }
    public ReportType Type { get; set; }
    public int? PostId { get; set; }
    public int? CommentId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? AdditionalDetails { get; set; }
    public ReportStatus Status { get; set; } = ReportStatus.Pending;
    public int? ResolvedByUserId { get; set; }
    public string? ResolutionNote { get; set; }
    public DateTime? ResolvedAt { get; set; }

    // Navigation Properties
    public User Reporter { get; set; } = null!;
    public Post? Post { get; set; }
    public Comment? Comment { get; set; }
    public User? ResolvedByUser { get; set; }

    // Domain Methods
    public void StartReview(int adminUserId)
    {
        if (Status != ReportStatus.Pending)
            throw new InvalidOperationException("Only pending reports can be moved to under review.");

        Status = ReportStatus.UnderReview;
        ResolvedByUserId = adminUserId;
        UpdateTimestamp();
    }

    public void Resolve(int adminUserId, string resolutionNote)
    {
        if (Status == ReportStatus.Resolved || Status == ReportStatus.Dismissed)
            throw new InvalidOperationException("Report is already resolved.");

        Status = ReportStatus.Resolved;
        ResolvedByUserId = adminUserId;
        ResolutionNote = resolutionNote;
        ResolvedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void Dismiss(int adminUserId, string reason)
    {
        if (Status == ReportStatus.Resolved || Status == ReportStatus.Dismissed)
            throw new InvalidOperationException("Report is already resolved.");

        Status = ReportStatus.Dismissed;
        ResolvedByUserId = adminUserId;
        ResolutionNote = reason;
        ResolvedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public bool IsPending()
    {
        return Status == ReportStatus.Pending;
    }

    public bool IsResolved()
    {
        return Status == ReportStatus.Resolved || Status == ReportStatus.Dismissed;
    }

    public string GetReportedContentType()
    {
        return Type == ReportType.Post ? "Post" : "Comment";
    }
}