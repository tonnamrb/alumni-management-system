namespace Domain.Entities;

/// <summary>
/// Extended profile information specific to alumni users
/// Supports FR-004, FR-005, FR-006, FR-007: Alumni Profile Management
/// Enhanced with flexible external data integration support
/// </summary>
public class AlumniProfile : BaseEntity
{
    // Foreign Key
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    // External Data Tracking
    public string? ExternalMemberID { get; set; }  // รหัสสมาชิกจากระบบเดิม
    public string? ExternalSystemId { get; set; }  // แหล่งที่มาของข้อมูล
    public DateTime? ExternalDataLastSync { get; set; }
    
    // Personal Information (Flexible String Fields)
    public string? NameInYearbook { get; set; }  // ชื่อในรุ่น
    public string? TitleCode { get; set; }  // "Mr.", "Mrs.", "Dr." etc.
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? NickName { get; set; }
    
    // Group & Classification
    public string? GroupCode { get; set; }  // รหัสกลุ่ม/สาขา
    public string? ClassName { get; set; }  // ชั้นปี/รุ่น
    public int? GraduationYear { get; set; }
    public string? Major { get; set; }  // Existing field maintained
    
    // Contact Information
    public string? Phone { get; set; }  // โทรศัพท์บ้าน/ที่ทำงาน
    public string? MobilePhone { get; set; }  // โทรศัพท์มือถือ (อาจซ้ำกับ User.MobilePhone)
    public string? PhoneNumber { get; set; }  // Existing field maintained for backward compatibility
    public string? LineID { get; set; }
    public string? Facebook { get; set; }
    public string? Email { get; set; }  // อาจซ้ำกับ User.Email
    public string? LinkedInProfile { get; set; }  // Existing field maintained
    
    // Address Information
    public string? Address { get; set; }  // ที่อยู่เต็ม
    public string? ZipCode { get; set; }
    public string? District { get; set; }  // ตำบล/แขวง
    public string? Province { get; set; }  // จังหวัด
    public string? Country { get; set; } = "Thailand";
    
    // Professional Information
    public string? CompanyName { get; set; }
    public string? CurrentCompany { get; set; }  // Existing field maintained for backward compatibility
    public string? JobTitle { get; set; }
    public string? CurrentJobTitle { get; set; }  // Existing field maintained for backward compatibility
    public string? WorkAddress { get; set; }
    
    // Personal Status
    public string? MaritalStatus { get; set; }  // "Single", "Married", "Divorced" etc.
    public string? SpouseName { get; set; }
    public string? Status { get; set; }  // สถานะสมาชิก
    
    // Additional Information
    public string? Comment { get; set; }  // หมายเหตุ
    public string? Bio { get; set; }  // Existing field maintained
    public DateTime? DateOfBirth { get; set; }
    public string? ProfilePictureUrl { get; set; }  // Existing field maintained
    public bool IsProfilePublic { get; set; } = true;  // Existing field maintained
    
    // Metadata
    public bool IsVerified { get; set; } = false;  // ยืนยันข้อมูลแล้ว
    public DateTime? ProfileCompletedAt { get; set; }

    // Domain Methods
    public void UpdateProfilePicture(string imageUrl)
    {
        ProfilePictureUrl = imageUrl;
        UpdateTimestamp();
    }

    public void MakeProfilePrivate()
    {
        IsProfilePublic = false;
        UpdateTimestamp();
    }

    public void MakeProfilePublic()
    {
        IsProfilePublic = true;
        UpdateTimestamp();
    }

    public bool HasCompletedProfile()
    {
        return !string.IsNullOrWhiteSpace(Bio) &&
               !string.IsNullOrWhiteSpace(GraduationYear?.ToString()) &&
               !string.IsNullOrWhiteSpace(Major);
    }

    public void MarkAsVerified()
    {
        IsVerified = true;
        ProfileCompletedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void UpdateExternalData(string externalMemberID, string externalSystemId)
    {
        ExternalMemberID = externalMemberID;
        ExternalSystemId = externalSystemId;
        ExternalDataLastSync = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public bool IsFromExternalSystem()
    {
        return !string.IsNullOrWhiteSpace(ExternalSystemId);
    }
}