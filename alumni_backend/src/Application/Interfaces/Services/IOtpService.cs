namespace Application.Interfaces.Services;

/// <summary>
/// Service interface for OTP operations
/// </summary>
public interface IOtpService
{
    /// <summary>
    /// Generate and save OTP for mobile phone registration
    /// </summary>
    /// <param name="mobilePhone">Mobile phone number</param>
    /// <param name="purpose">Purpose of OTP (registration, login, etc.)</param>
    /// <returns>Generated OTP code (for development - remove in production)</returns>
    Task<string> GenerateOtpAsync(string mobilePhone, string purpose = "registration");
    
    /// <summary>
    /// Verify OTP code for mobile phone
    /// </summary>
    /// <param name="mobilePhone">Mobile phone number</param>
    /// <param name="otpCode">OTP code to verify</param>
    /// <param name="purpose">Purpose of OTP verification</param>
    /// <returns>True if OTP is valid</returns>
    Task<bool> VerifyOtpAsync(string mobilePhone, string otpCode, string purpose = "registration");
    
    /// <summary>
    /// Check if mobile phone has reached OTP request rate limit
    /// </summary>
    /// <param name="mobilePhone">Mobile phone number</param>
    /// <returns>True if rate limit reached</returns>
    Task<bool> HasReachedRateLimitAsync(string mobilePhone);
    
    /// <summary>
    /// Clean up expired OTPs (background job)
    /// </summary>
    /// <returns>Number of deleted OTPs</returns>
    Task<int> CleanupExpiredOtpsAsync();
}