using Domain.Entities;
using System.Security.Claims;

namespace Application.Interfaces.Services;

public interface IJwtTokenService
{
    /// <summary>
    /// สร้าง JWT access token สำหรับ user
    /// </summary>
    /// <param name="user">ข้อมูล user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>JWT token string</returns>
    Task<string> GenerateAccessTokenAsync(User user, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// สร้าง refresh token
    /// </summary>
    /// <returns>Refresh token string</returns>
    string GenerateRefreshToken();
    
    /// <summary>
    /// ตรวจสอบความถูกต้องของ JWT token
    /// </summary>
    /// <param name="token">JWT token string</param>
    /// <returns>ClaimsPrincipal หากถูกต้อง, null หากไม่ถูกต้อง</returns>
    ClaimsPrincipal? ValidateToken(string token);
    
    /// <summary>
    /// ดึง user ID จาก JWT token
    /// </summary>
    /// <param name="token">JWT token string</param>
    /// <returns>User ID หากถูกต้อง, null หากไม่ถูกต้อง</returns>
    int? GetUserIdFromToken(string token);
    
    /// <summary>
    /// ดึง user email จาก JWT token
    /// </summary>
    /// <param name="token">JWT token string</param>
    /// <returns>Email หากถูกต้อง, null หากไม่ถูกต้อง</returns>
    string? GetEmailFromToken(string token);
    
    /// <summary>
    /// ดึง user role จาก JWT token
    /// </summary>
    /// <param name="token">JWT token string</param>
    /// <returns>Role หากถูกต้อง, null หากไม่ถูกต้อง</returns>
    string? GetRoleFromToken(string token);
    
    /// <summary>
    /// ตรวจสอบว่า token หมดอายุหรือยัง
    /// </summary>
    /// <param name="token">JWT token string</param>
    /// <returns>หมดอายุหรือไม่</returns>
    bool IsTokenExpired(string token);
    
    /// <summary>
    /// ดึงเวลาหมดอายุของ token
    /// </summary>
    /// <param name="token">JWT token string</param>
    /// <returns>เวลาหมดอายุ หากถูกต้อง, null หากไม่ถูกต้อง</returns>
    DateTime? GetTokenExpiration(string token);
}