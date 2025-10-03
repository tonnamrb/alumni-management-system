namespace Application.DTOs;

public class UserDto
{
    public int Id { get; set; }
    
    // Core Authentication Fields
    public string Email { get; set; } = string.Empty;
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? MobilePhone { get; set; }
    
    // Role Management
    public int RoleId { get; set; }
    public string? RoleName { get; set; }
    public bool IsDefaultAdmin { get; set; }
    
    // Alumni Member Specific Fields (when RoleId = 1)
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
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation Properties
    public AlumniProfileDto? AlumniProfile { get; set; }
    
    // Computed Properties
    public bool IsMember => RoleId == 1;
    public bool IsAdmin => RoleId == 2;
}

public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Provider { get; set; }
    public string? ProviderId { get; set; }
}

public class UpdateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = new();
}

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}