using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Represents both alumni users and administrators with different permission levels
/// Supports FR-001, FR-002, FR-003: Authentication & User Management
/// Enhanced with mobile phone authentication and external data integration support
/// </summary>
public class User : BaseEntity
{
    // Core Authentication Fields
    public string MobilePhone { get; set; } = string.Empty;  // Primary login field
    public string? Email { get; set; }  // Optional secondary contact
    public string Name { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    
    // External System Integration
    public string? ExternalMemberID { get; set; }  // รหัสสมาชิกจากระบบเดิม
    public string? ExternalSystemId { get; set; }  // ระบุแหล่งที่มาของข้อมูล
    public DateTime? ExternalDataLastSync { get; set; }  // วันที่ sync ล่าสุด
    
    // Authentication & Security
    public string? Provider { get; set; } // "Mobile", "Google", "Facebook", "External"
    public string? ProviderId { get; set; }
    public string? PictureUrl { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public AlumniProfile? AlumniProfile { get; set; }
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Report> Reports { get; set; } = new List<Report>(); // Reports created by this user
    public ICollection<Report> ResolvedReports { get; set; } = new List<Report>(); // Reports resolved by this admin

    // Domain Methods
    public bool IsAlumni() => Role == UserRole.Alumni;
    public bool IsAdministrator() => Role == UserRole.Administrator;
    
    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdateTimestamp();
    }

    public void Activate()
    {
        IsActive = true;
        UpdateTimestamp();
    }

    // Mobile Authentication Methods
    public bool HasMobilePhone() => !string.IsNullOrWhiteSpace(MobilePhone);
    
    public void UpdateMobilePhone(string mobilePhone)
    {
        MobilePhone = mobilePhone;
        UpdateTimestamp();
    }

    // External Data Methods
    public void UpdateExternalData(string externalMemberID, string externalSystemId)
    {
        ExternalMemberID = externalMemberID;
        ExternalSystemId = externalSystemId;
        ExternalDataLastSync = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public bool IsFromExternalSystem() => !string.IsNullOrWhiteSpace(ExternalSystemId);
    
    public bool IsMobileUser() => Provider == "Mobile";
    
    public bool IsExternalUser() => Provider == "External";
}