using Application.DTOs.Auth;
using Domain.Entities;

namespace Application.Interfaces.Services;

/// <summary>
/// Interface for authentication operations including mobile phone authentication
/// </summary>
public interface IAuthenticationService
{
    // Email authentication (existing)
    Task<AuthResult> LoginWithEmailAsync(string email, string password);
    Task<User> RegisterWithEmailAsync(string email, string password, string name);
    
    // Mobile phone authentication (updated for backoffice integration)
    Task<AuthResult> LoginWithMobilePhoneAsync(string mobilePhone, string password);
    
    // Registration flow with OTP verification
    Task<bool> CanRegisterWithMobilePhoneAsync(string mobilePhone);
    Task<bool> RequestRegistrationOtpAsync(string mobilePhone);
    Task<bool> VerifyRegistrationOtpAsync(string mobilePhone, string otpCode);
    Task<User> CompleteRegistrationAsync(string mobilePhone, string password);
    
    // External authentication
    Task<AuthResult> LoginWithProviderAsync(string provider, string providerId, string email, string name, string? pictureUrl = null);
    
    // OTP operations (legacy - keep for compatibility)
    Task<bool> RequestOtpAsync(string mobilePhone, string purpose = "login");
    Task<bool> VerifyOtpAsync(string mobilePhone, string otpCode);
    
    // Token operations
    Task<AuthResult> RefreshTokenAsync(string token);
    Task<bool> RevokeTokenAsync(string token);
    
    // Password operations (for email-based authentication)
    Task<bool> ResetPasswordAsync(string email);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
}