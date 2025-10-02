using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(
        IAuditLogRepository auditLogRepository,
        ILogger<AuditLogService> logger)
    {
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    public async Task<AuditLog> LogAsync(
        int? userId,
        AuditAction action,
        string entityType,
        int entityId,
        object? oldValues = null,
        object? newValues = null,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues, GetJsonOptions()) : null,
                NewValues = newValues != null ? JsonSerializer.Serialize(newValues, GetJsonOptions()) : null,
                Timestamp = DateTime.UtcNow,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            var savedAuditLog = await _auditLogRepository.AddAsync(auditLog, cancellationToken);
            await _auditLogRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Audit log created: UserId={UserId}, Action={Action}, EntityType={EntityType}, EntityId={EntityId}",
                userId, action, entityType, entityId);

            return savedAuditLog;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error creating audit log: UserId={UserId}, Action={Action}, EntityType={EntityType}, EntityId={EntityId}",
                userId, action, entityType, entityId);
            throw;
        }
    }

    public async Task<AuditLog> LogCreateAsync(
        int userId,
        string entityType,
        int entityId,
        object newValues,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        return await LogAsync(
            userId,
            AuditAction.Create,
            entityType,
            entityId,
            oldValues: null,
            newValues: newValues,
            ipAddress: ipAddress,
            userAgent: userAgent,
            cancellationToken: cancellationToken);
    }

    public async Task<AuditLog> LogUpdateAsync(
        int userId,
        string entityType,
        int entityId,
        object oldValues,
        object newValues,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        return await LogAsync(
            userId,
            AuditAction.Update,
            entityType,
            entityId,
            oldValues: oldValues,
            newValues: newValues,
            ipAddress: ipAddress,
            userAgent: userAgent,
            cancellationToken: cancellationToken);
    }

    public async Task<AuditLog> LogDeleteAsync(
        int userId,
        string entityType,
        int entityId,
        object oldValues,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        return await LogAsync(
            userId,
            AuditAction.Delete,
            entityType,
            entityId,
            oldValues: oldValues,
            newValues: null,
            ipAddress: ipAddress,
            userAgent: userAgent,
            cancellationToken: cancellationToken);
    }

    public async Task<AuditLog> LogLoginAsync(
        int userId,
        string? ipAddress = null,
        string? userAgent = null,
        bool success = true,
        CancellationToken cancellationToken = default)
    {
        var action = success ? AuditAction.Login : AuditAction.LoginFailed;
        
        return await LogAsync(
            userId,
            action,
            "User",
            userId,
            oldValues: null,
            newValues: new { LoginTime = DateTime.UtcNow, Success = success },
            ipAddress: ipAddress,
            userAgent: userAgent,
            cancellationToken: cancellationToken);
    }

    private static JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }
}