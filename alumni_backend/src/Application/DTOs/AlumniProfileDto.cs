namespace Application.DTOs;

public class AlumniProfileDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? GraduationYear { get; set; }
    public string? Major { get; set; }
    public string? CurrentJobTitle { get; set; }
    public string? CurrentCompany { get; set; }
    public string? PhoneNumber { get; set; }
    public string? LinkedInProfile { get; set; }
    public bool IsProfilePublic { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation Properties
    public UserDto? User { get; set; }
}

public class CreateAlumniProfileDto
{
    public string? Bio { get; set; }
    public string? GraduationYear { get; set; }
    public string? Major { get; set; }
    public string? CurrentJobTitle { get; set; }
    public string? CurrentCompany { get; set; }
    public string? PhoneNumber { get; set; }
    public string? LinkedInProfile { get; set; }
    public bool IsProfilePublic { get; set; } = true;
}

public class UpdateAlumniProfileDto
{
    public string? Bio { get; set; }
    public string? GraduationYear { get; set; }
    public string? Major { get; set; }
    public string? CurrentJobTitle { get; set; }
    public string? CurrentCompany { get; set; }
    public string? PhoneNumber { get; set; }
    public string? LinkedInProfile { get; set; }
    public bool IsProfilePublic { get; set; }
}

public class ProfileSearchDto
{
    public string? Major { get; set; }
    public string? GraduationYear { get; set; }
    public string? Company { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}