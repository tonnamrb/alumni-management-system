namespace Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for OTP operations
/// </summary>
public interface IOtpRepository : IBaseRepository<Domain.Entities.Otp>
{
    /// <summary>
    /// Get active (unexpired, unused) OTP for a mobile phone and purpose
    /// </summary>
    Task<Domain.Entities.Otp?> GetActiveOtpAsync(string mobilePhone, string purpose);
    
    /// <summary>
    /// Get the most recent OTP for a mobile phone and purpose
    /// </summary>
    Task<Domain.Entities.Otp?> GetLatestOtpAsync(string mobilePhone, string purpose);
    
    /// <summary>
    /// Delete expired OTPs (cleanup job)
    /// </summary>
    Task<int> DeleteExpiredOtpsAsync();
    
    /// <summary>
    /// Count active OTPs for a mobile phone within a time period (rate limiting)
    /// </summary>
    Task<int> CountActiveOtpsAsync(string mobilePhone, TimeSpan timeWindow);
    
    /// <summary>
    /// Check if mobile phone has reached rate limit for OTP requests
    /// </summary>
    Task<bool> HasReachedRateLimitAsync(string mobilePhone, int maxRequests, TimeSpan timeWindow);
}