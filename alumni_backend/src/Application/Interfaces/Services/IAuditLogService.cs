using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface IAuditLogService
{
    /// <summary>
    /// บันทึก audit log สำหรับการกระทำของ user
    /// </summary>
    /// <param name="userId">ID ของ user ที่ทำการกระทำ (null หากเป็น system action)</param>
    /// <param name="action">ประเภทของการกระทำ</param>
    /// <param name="entityType">ประเภทของ entity ที่ถูกกระทำ</param>
    /// <param name="entityId">ID ของ entity ที่ถูกกระทำ</param>
    /// <param name="oldValues">ค่าเก่าของ entity (JSON format)</param>
    /// <param name="newValues">ค่าใหม่ของ entity (JSON format)</param>
    /// <param name="ipAddress">IP address ของ user</param>
    /// <param name="userAgent">User agent ของ browser</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AuditLog ที่สร้างขึ้น</returns>
    Task<AuditLog> LogAsync(
        int? userId,
        AuditAction action,
        string entityType,
        int entityId,
        object? oldValues = null,
        object? newValues = null,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// บันทึก audit log สำหรับการสร้าง entity ใหม่
    /// </summary>
    /// <param name="userId">ID ของ user ที่สร้าง</param>
    /// <param name="entityType">ประเภทของ entity</param>
    /// <param name="entityId">ID ของ entity ที่สร้าง</param>
    /// <param name="newValues">ค่าของ entity ที่สร้าง</param>
    /// <param name="ipAddress">IP address</param>
    /// <param name="userAgent">User agent</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AuditLog ที่สร้างขึ้น</returns>
    Task<AuditLog> LogCreateAsync(
        int userId,
        string entityType,
        int entityId,
        object newValues,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// บันทึก audit log สำหรับการอัพเดต entity
    /// </summary>
    /// <param name="userId">ID ของ user ที่อัพเดต</param>
    /// <param name="entityType">ประเภทของ entity</param>
    /// <param name="entityId">ID ของ entity ที่อัพเดต</param>
    /// <param name="oldValues">ค่าเก่าของ entity</param>
    /// <param name="newValues">ค่าใหม่ของ entity</param>
    /// <param name="ipAddress">IP address</param>
    /// <param name="userAgent">User agent</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AuditLog ที่สร้างขึ้น</returns>
    Task<AuditLog> LogUpdateAsync(
        int userId,
        string entityType,
        int entityId,
        object oldValues,
        object newValues,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// บันทึก audit log สำหรับการลบ entity
    /// </summary>
    /// <param name="userId">ID ของ user ที่ลบ</param>
    /// <param name="entityType">ประเภทของ entity</param>
    /// <param name="entityId">ID ของ entity ที่ลบ</param>
    /// <param name="oldValues">ค่าของ entity ที่ลบ</param>
    /// <param name="ipAddress">IP address</param>
    /// <param name="userAgent">User agent</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AuditLog ที่สร้างขึ้น</returns>
    Task<AuditLog> LogDeleteAsync(
        int userId,
        string entityType,
        int entityId,
        object oldValues,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// บันทึก audit log สำหรับการ login
    /// </summary>
    /// <param name="userId">ID ของ user ที่ login</param>
    /// <param name="ipAddress">IP address</param>
    /// <param name="userAgent">User agent</param>
    /// <param name="success">login สำเร็จหรือไม่</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AuditLog ที่สร้างขึ้น</returns>
    Task<AuditLog> LogLoginAsync(
        int userId,
        string? ipAddress = null,
        string? userAgent = null,
        bool success = true,
        CancellationToken cancellationToken = default);
}