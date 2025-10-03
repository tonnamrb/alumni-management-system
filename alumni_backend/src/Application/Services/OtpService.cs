using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Helpers;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Service for OTP operations including generation, verification, and cleanup
/// </summary>
public class OtpService : IOtpService
{
    private readonly IOtpRepository _otpRepository;
    private readonly ILogger<OtpService> _logger;
    
    // Rate limiting configuration
    private const int MaxOtpRequestsPerHour = 5;
    private static readonly TimeSpan RateLimitWindow = TimeSpan.FromHours(1);
    
    // OTP configuration
    private const int OtpExpirationMinutes = 5;
    private const int MaxAttempts = 3;

    public OtpService(
        IOtpRepository otpRepository,
        ILogger<OtpService> logger)
    {
        _otpRepository = otpRepository;
        _logger = logger;
    }

    /// <summary>
    /// Generate and save OTP for mobile phone registration
    /// </summary>
    public async Task<string> GenerateOtpAsync(string mobilePhone, string purpose = "registration")
    {
        try
        {
            var normalizedPhone = PhoneNumberHelper.NormalizeMobilePhone(mobilePhone);
            
            // Check rate limit
            if (await HasReachedRateLimitAsync(normalizedPhone))
            {
                _logger.LogWarning("Rate limit reached for mobile phone: {MobilePhone}", normalizedPhone);
                throw new InvalidOperationException("คุณได้ขอรหัส OTP เกินขำกัดแล้ว กรุณาลองใหม่ในภายหลัง");
            }
            
            // Generate 6-digit OTP
            var otpCode = GenerateRandomOtpCode();
            
            // Create OTP entity
            var otp = new Domain.Entities.Otp
            {
                MobilePhone = normalizedPhone,
                Code = otpCode,
                Purpose = purpose,
                ExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpirationMinutes),
                MaxAttempts = MaxAttempts
            };
            
            // Save to database
            await _otpRepository.AddAsync(otp);
            await _otpRepository.SaveChangesAsync();
            
            _logger.LogInformation("OTP generated for phone: {MobilePhone}, Purpose: {Purpose}, Code: {Code}", 
                normalizedPhone, purpose, otpCode);
            
            // Return OTP code for development (should be sent via SMS in production)
            return otpCode;
        }
        catch (Exception ex) when (!(ex is InvalidOperationException))
        {
            _logger.LogError(ex, "Error generating OTP for phone: {MobilePhone}", mobilePhone);
            throw new InvalidOperationException("เกิดข้อผิดพลาดในการสร้างรหัส OTP");
        }
    }

    /// <summary>
    /// Verify OTP code for mobile phone
    /// </summary>
    public async Task<bool> VerifyOtpAsync(string mobilePhone, string otpCode, string purpose = "registration")
    {
        try
        {
            var normalizedPhone = PhoneNumberHelper.NormalizeMobilePhone(mobilePhone);
            
            // Get active OTP
            var otp = await _otpRepository.GetActiveOtpAsync(normalizedPhone, purpose);
            
            if (otp == null)
            {
                _logger.LogWarning("No active OTP found for phone: {MobilePhone}, Purpose: {Purpose}", 
                    normalizedPhone, purpose);
                return false;
            }
            
            // Check if OTP can still be used
            if (!otp.CanBeUsed())
            {
                _logger.LogWarning("OTP cannot be used for phone: {MobilePhone}, IsUsed: {IsUsed}, IsExpired: {IsExpired}, AttemptCount: {AttemptCount}", 
                    normalizedPhone, otp.IsUsed, otp.IsExpired(), otp.AttemptCount);
                return false;
            }
            
            // Verify OTP code
            if (otp.Code != otpCode)
            {
                // Increment attempt count
                otp.IncrementAttempt();
                await _otpRepository.UpdateAsync(otp);
                await _otpRepository.SaveChangesAsync();
                
                _logger.LogWarning("Invalid OTP code for phone: {MobilePhone}, Attempt: {AttemptCount}", 
                    normalizedPhone, otp.AttemptCount);
                return false;
            }
            
            // Mark OTP as used
            otp.MarkAsUsed();
            await _otpRepository.UpdateAsync(otp);
            await _otpRepository.SaveChangesAsync();
            
            _logger.LogInformation("OTP verified successfully for phone: {MobilePhone}, Purpose: {Purpose}", 
                normalizedPhone, purpose);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying OTP for phone: {MobilePhone}", mobilePhone);
            return false;
        }
    }

    /// <summary>
    /// Check if mobile phone has reached OTP request rate limit
    /// </summary>
    public async Task<bool> HasReachedRateLimitAsync(string mobilePhone)
    {
        try
        {
            var normalizedPhone = PhoneNumberHelper.NormalizeMobilePhone(mobilePhone);
            return await _otpRepository.HasReachedRateLimitAsync(normalizedPhone, MaxOtpRequestsPerHour, RateLimitWindow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking rate limit for phone: {MobilePhone}", mobilePhone);
            // Return false to allow OTP generation in case of error
            return false;
        }
    }

    /// <summary>
    /// Clean up expired OTPs (background job)
    /// </summary>
    public async Task<int> CleanupExpiredOtpsAsync()
    {
        try
        {
            var deletedCount = await _otpRepository.DeleteExpiredOtpsAsync();
            
            if (deletedCount > 0)
            {
                _logger.LogInformation("Cleaned up {Count} expired OTPs", deletedCount);
            }
            
            return deletedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired OTPs");
            return 0;
        }
    }

    /// <summary>
    /// Generate a random 6-digit OTP code
    /// </summary>
    private string GenerateRandomOtpCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}