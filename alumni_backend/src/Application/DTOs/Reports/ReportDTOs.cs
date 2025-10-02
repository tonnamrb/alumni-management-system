using System.ComponentModel.DataAnnotations;
using Application.DTOs;
using Domain.Enums;

namespace Application.DTOs.Reports;

public class CreateReportDto
{
    [Required]
    public ReportType Type { get; set; }
    
    public int? PostId { get; set; }
    
    public int? CommentId { get; set; }
    
    [Required]
    [StringLength(1000, ErrorMessage = "เหตุผลการรายงานต้องไม่เกิน 1000 ตัวอักษร")]
    public string Reason { get; set; } = string.Empty;
    
    [StringLength(2000, ErrorMessage = "รายละเอียดเพิ่มเติมต้องไม่เกิน 2000 ตัวอักษร")]
    public string? AdditionalDetails { get; set; }
}

public class ReportDto
{
    public int Id { get; set; }
    public int ReporterId { get; set; }
    public ReportType Type { get; set; }
    public int? PostId { get; set; }
    public int? CommentId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? AdditionalDetails { get; set; }
    public ReportStatus Status { get; set; }
    public int? ResolvedByUserId { get; set; }
    public string? ResolutionNote { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    
    // Navigation Properties
    public UserDto Reporter { get; set; } = new();
    public UserDto? ResolvedByUser { get; set; }
    public string? ReportedContent { get; set; } // Post content or Comment content
    public string? ReportedUserName { get; set; } // Name of user who created the reported content
}

public class ReportListDto
{
    public List<ReportDto> Reports { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}

public class ResolveReportDto
{
    [Required]
    public ReportStatus Status { get; set; } = ReportStatus.Resolved;
    
    [Required]
    [StringLength(1000, ErrorMessage = "หมายเหตุการแก้ไขต้องไม่เกิน 1000 ตัวอักษร")]
    public string ResolutionNote { get; set; } = string.Empty;
}