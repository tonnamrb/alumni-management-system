using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Unified User and Member Data entity that maps to the "Users" table
/// Contains both basic user information and alumni member-specific fields
/// Follows the schema designed by the backoffice team
/// </summary>
public class User : BaseEntity
{
    // Primary key is Guid in the backoffice schema, but we'll use int and let them handle mapping
    
    // Core Authentication Fields (Required)
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public string? MobilePhone { get; set; }
    
    // Role Management (Foreign Key to Roles table)
    public int RoleId { get; set; } = 1; // Default to Member (RoleId = 1)
    public bool IsDefaultAdmin { get; set; } = false;
    
    // Alumni Member Specific Fields (nullable - only used when RoleId = 1 Member)
    public string? MemberID { get; set; }
    public string? NameInYearbook { get; set; }
    public string? TitleID { get; set; }
    public string? NickName { get; set; }
    public string? GroupID { get; set; }
    public string? Phone { get; set; }
    public string? LineID { get; set; }
    public string? Facebook { get; set; }
    public string? Address { get; set; }
    public string? ZipCode { get; set; }
    public string? CompanyName { get; set; }
    public string? Status { get; set; }
    public string? SpouseName { get; set; }
    public string? Comment { get; set; }

    // Navigation Properties
    public Role? Role { get; set; }
    public AlumniProfile? AlumniProfile { get; set; }
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Report> Reports { get; set; } = new List<Report>(); // Reports created by this user
    public ICollection<Report> ResolvedReports { get; set; } = new List<Report>(); // Reports resolved by this admin

    // Computed Properties
    public string FullName => $"{Firstname} {Lastname}".Trim();
    public bool IsMember => RoleId == 1;
    public bool IsAdmin => RoleId == 2;
    
    // Domain Methods
    public bool IsAlumni() => RoleId == 1; // Member role
    public bool IsAdministrator() => RoleId == 2; // Admin role
    
    // Mobile Authentication Methods
    public bool HasMobilePhone() => !string.IsNullOrWhiteSpace(MobilePhone);
    
    public void UpdateMobilePhone(string? mobilePhone)
    {
        MobilePhone = mobilePhone;
        UpdateTimestamp();
    }
    
    public void UpdateName(string firstname, string lastname)
    {
        Firstname = firstname;
        Lastname = lastname;
        UpdateTimestamp();
    }
    
    // Member-specific methods (only applicable when RoleId = 1)
    public void UpdateMemberData(string? memberID, string? nameInYearbook, string? groupID)
    {
        if (RoleId == 1) // Only for Members
        {
            MemberID = memberID;
            NameInYearbook = nameInYearbook;
            GroupID = groupID;
            UpdateTimestamp();
        }
    }
    
    public void UpdateContactInfo(string? phone, string? lineId, string? facebook)
    {
        if (RoleId == 1) // Only for Members
        {
            Phone = phone;
            LineID = lineId;
            Facebook = facebook;
            UpdateTimestamp();
        }
    }
    
    public void UpdateAddress(string? address, string? zipCode)
    {
        if (RoleId == 1) // Only for Members
        {
            Address = address;
            ZipCode = zipCode;
            UpdateTimestamp();
        }
    }
    
    public void UpdateWorkInfo(string? companyName)
    {
        if (RoleId == 1) // Only for Members
        {
            CompanyName = companyName;
            UpdateTimestamp();
        }
    }
    
    public void UpdateStatus(string? status)
    {
        if (RoleId == 1) // Only for Members
        {
            Status = status;
            UpdateTimestamp();
        }
    }
}