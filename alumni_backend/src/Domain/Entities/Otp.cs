namespace Domain.Entities;

/// <summary>
/// OTP (One-Time Password) entity for mobile phone verification
/// Used for registration and authentication processes
/// </summary>
public class Otp : BaseEntity
{
    /// <summary>
    /// Mobile phone number in normalized format (e.g., "66812345678")
    /// </summary>
    public string MobilePhone { get; set; } = string.Empty;
    
    /// <summary>
    /// 6-digit OTP code
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Expiration timestamp for the OTP
    /// </summary>
    public DateTime ExpiresAt { get; set; }
    
    /// <summary>
    /// Purpose of the OTP: "registration", "login", "password_reset"
    /// </summary>
    public string Purpose { get; set; } = "registration";
    
    /// <summary>
    /// Whether the OTP has been used
    /// </summary>
    public bool IsUsed { get; set; } = false;
    
    /// <summary>
    /// Timestamp when the OTP was used
    /// </summary>
    public DateTime? UsedAt { get; set; }
    
    /// <summary>
    /// Number of verification attempts
    /// </summary>
    public int AttemptCount { get; set; } = 0;
    
    /// <summary>
    /// Maximum allowed verification attempts
    /// </summary>
    public int MaxAttempts { get; set; } = 3;
    
    // Domain Methods
    
    /// <summary>
    /// Check if the OTP has expired
    /// </summary>
    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;
    
    /// <summary>
    /// Check if the OTP can still be used
    /// </summary>
    public bool CanBeUsed() => !IsUsed && !IsExpired() && AttemptCount < MaxAttempts;
    
    /// <summary>
    /// Mark the OTP as used
    /// </summary>
    public void MarkAsUsed()
    {
        IsUsed = true;
        UsedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }
    
    /// <summary>
    /// Increment attempt count
    /// </summary>
    public void IncrementAttempt()
    {
        AttemptCount++;
        UpdateTimestamp();
    }
    
    /// <summary>
    /// Check if maximum attempts have been reached
    /// </summary>
    public bool HasReachedMaxAttempts() => AttemptCount >= MaxAttempts;
}