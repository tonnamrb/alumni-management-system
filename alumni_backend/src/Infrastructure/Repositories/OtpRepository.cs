using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Repository implementation for OTP operations
/// </summary>
public class OtpRepository : BaseRepository<Domain.Entities.Otp>, IOtpRepository
{
    public OtpRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get active (unexpired, unused) OTP for a mobile phone and purpose
    /// </summary>
    public async Task<Domain.Entities.Otp?> GetActiveOtpAsync(string mobilePhone, string purpose)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .FirstOrDefaultAsync(o => 
                o.MobilePhone == mobilePhone && 
                o.Purpose == purpose && 
                !o.IsUsed && 
                o.ExpiresAt > now, CancellationToken.None);
    }

    /// <summary>
    /// Get latest OTP for a mobile phone and purpose (regardless of status)
    /// </summary>
    public async Task<Domain.Entities.Otp?> GetLatestOtpAsync(string mobilePhone, string purpose)
    {
        return await _dbSet
            .Where(o => o.MobilePhone == mobilePhone && o.Purpose == purpose)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(CancellationToken.None);
    }

    /// <summary>
    /// Count OTPs sent to mobile phone within time period
    /// </summary>
    public async Task<int> CountOtpsSentInPeriodAsync(string mobilePhone, string purpose, DateTime since)
    {
        return await _dbSet
            .CountAsync(o => 
                o.MobilePhone == mobilePhone && 
                o.Purpose == purpose && 
                o.CreatedAt >= since, CancellationToken.None);
    }

    /// <summary>
    /// Clean up expired OTPs older than specified date
    /// </summary>
    public async Task<int> CleanupExpiredOtpsAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var expiredOtps = await _dbSet
            .Where(o => o.ExpiresAt < olderThan)
            .ToListAsync(cancellationToken);

        if (expiredOtps.Any())
        {
            _dbSet.RemoveRange(expiredOtps);
            await SaveChangesAsync(cancellationToken);
        }

        return expiredOtps.Count;
    }

    /// <summary>
    /// Get OTP statistics for monitoring
    /// </summary>
    public async Task<Dictionary<string, int>> GetOtpStatisticsAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        var stats = await _dbSet
            .Where(o => o.CreatedAt >= since)
            .GroupBy(o => o.Purpose)
            .Select(g => new { Purpose = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Purpose, x => x.Count, cancellationToken);

        return stats;
    }

    /// <summary>
    /// Delete expired OTPs (cleanup job)
    /// </summary>
    public async Task<int> DeleteExpiredOtpsAsync()
    {
        var now = DateTime.UtcNow;
        var expiredOtps = await _dbSet
            .Where(o => o.ExpiresAt < now)
            .ToListAsync();

        if (expiredOtps.Any())
        {
            _dbSet.RemoveRange(expiredOtps);
            await SaveChangesAsync(CancellationToken.None);
        }

        return expiredOtps.Count;
    }

    /// <summary>
    /// Count active OTPs for a mobile phone within a time period (rate limiting)
    /// </summary>
    public async Task<int> CountActiveOtpsAsync(string mobilePhone, TimeSpan timeWindow)
    {
        var since = DateTime.UtcNow.Subtract(timeWindow);
        return await _dbSet
            .CountAsync(o => 
                o.MobilePhone == mobilePhone && 
                o.CreatedAt >= since);
    }

    /// <summary>
    /// Check if mobile phone has reached rate limit for OTP requests
    /// </summary>
    public async Task<bool> HasReachedRateLimitAsync(string mobilePhone, int maxRequests, TimeSpan timeWindow)
    {
        var count = await CountActiveOtpsAsync(mobilePhone, timeWindow);
        return count >= maxRequests;
    }
}