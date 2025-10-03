using System.ComponentModel.DataAnnotations;
using Application.Helpers;

namespace Application.DTOs.Auth;

/// <summary>
/// Request model for mobile phone login
/// </summary>
public class MobileLoginRequest
{
    [Required(ErrorMessage = "Mobile phone number is required")]
    [StringLength(15, ErrorMessage = "Mobile phone number must be between 8 and 15 characters", MinimumLength = 8)]
    public string MobilePhone { get; set; } = string.Empty;
    
    /// <summary>
    /// Password for mobile phone login authentication
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, ErrorMessage = "Password must be between 6 and 100 characters", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// Validates the mobile phone format
    /// </summary>
    /// <returns>Validation result</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!PhoneNumberHelper.IsValidThaiMobilePhone(MobilePhone))
        {
            yield return new ValidationResult(
                "Invalid Thai mobile phone number format. Expected format: 06xxxxxxxx, 08xxxxxxxx, or 09xxxxxxxx",
                new[] { nameof(MobilePhone) });
        }
    }
}

/// <summary>
/// Request model for mobile phone registration
/// </summary>
public class MobileRegisterRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name must be between 2 and 100 characters", MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Mobile phone number is required")]
    [StringLength(15, ErrorMessage = "Mobile phone number must be between 8 and 15 characters", MinimumLength = 8)]
    public string MobilePhone { get; set; } = string.Empty;
    
    /// <summary>
    /// Password for mobile phone registration
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, ErrorMessage = "Password must be between 6 and 100 characters", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional email for additional contact
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }
    
    /// <summary>
    /// Validates the mobile phone format
    /// </summary>
    /// <returns>Validation result</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!PhoneNumberHelper.IsValidThaiMobilePhone(MobilePhone))
        {
            yield return new ValidationResult(
                "Invalid Thai mobile phone number format. Expected format: 06xxxxxxxx, 08xxxxxxxx, or 09xxxxxxxx",
                new[] { nameof(MobilePhone) });
        }
    }
}

/// <summary>
/// Response model for authentication operations
/// </summary>
public class AuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public UserDto? User { get; set; }
    public string? Error { get; set; }
    public DateTime? ExpiresAt { get; set; }
    
    public static AuthResult Successful(string token, UserDto user, DateTime? expiresAt = null)
    {
        return new AuthResult
        {
            Success = true,
            Token = token,
            User = user,
            ExpiresAt = expiresAt
        };
    }
    
    public static AuthResult Failed(string error)
    {
        return new AuthResult
        {
            Success = false,
            Error = error
        };
    }
}

/// <summary>
/// Request model for OTP verification (future implementation)
/// </summary>
public class OtpVerificationRequest
{
    [Required(ErrorMessage = "Mobile phone number is required")]
    public string MobilePhone { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "OTP code is required")]
    [StringLength(6, ErrorMessage = "OTP must be 6 digits", MinimumLength = 6)]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be 6 digits")]
    public string OtpCode { get; set; } = string.Empty;
}

/// <summary>
/// Request model for requesting OTP (future implementation)  
/// </summary>
public class RequestOtpRequest
{
    [Required(ErrorMessage = "Mobile phone number is required")]
    public string MobilePhone { get; set; } = string.Empty;
    
    /// <summary>
    /// Purpose of OTP: "login", "register", "reset_password"
    /// </summary>
    [Required(ErrorMessage = "Purpose is required")]
    public string Purpose { get; set; } = "login";
}

/// <summary>
/// Request model for checking if mobile phone can register
/// </summary>
public class CheckMobileRequest
{
    [Required(ErrorMessage = "Mobile phone number is required")]
    [StringLength(15, ErrorMessage = "Mobile phone number must be between 8 and 15 characters", MinimumLength = 8)]
    public string MobilePhone { get; set; } = string.Empty;
    
    /// <summary>
    /// Validates the mobile phone format
    /// </summary>
    /// <returns>Validation result</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!PhoneNumberHelper.IsValidThaiMobilePhone(MobilePhone))
        {
            yield return new ValidationResult(
                "Invalid Thai mobile phone number format. Expected format: 06xxxxxxxx, 08xxxxxxxx, or 09xxxxxxxx",
                new[] { nameof(MobilePhone) });
        }
    }
}

/// <summary>
/// Request model for completing registration after OTP verification
/// </summary>
public class CompleteRegistrationRequest
{
    [Required(ErrorMessage = "Mobile phone number is required")]
    [StringLength(15, ErrorMessage = "Mobile phone number must be between 8 and 15 characters", MinimumLength = 8)]
    public string MobilePhone { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, ErrorMessage = "Password must be between 6 and 100 characters", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// Validates the mobile phone format
    /// </summary>
    /// <returns>Validation result</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!PhoneNumberHelper.IsValidThaiMobilePhone(MobilePhone))
        {
            yield return new ValidationResult(
                "Invalid Thai mobile phone number format. Expected format: 06xxxxxxxx, 08xxxxxxxx, or 09xxxxxxxx",
                new[] { nameof(MobilePhone) });
        }
    }
}