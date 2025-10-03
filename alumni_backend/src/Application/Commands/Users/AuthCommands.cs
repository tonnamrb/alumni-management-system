using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Application.Commands.Users;

// Login Command
public record LoginCommand(LoginDto LoginDto, string? IpAddress = null, string? UserAgent = null) : IRequest<LoginResponseDto>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<LoginCommandHandler> _logger;
    private readonly IAuditLogService _auditLogService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IMapper mapper,
        ILogger<LoginCommandHandler> logger,
        IAuditLogService auditLogService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
        _logger = logger;
        _auditLogService = auditLogService;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(request.LoginDto.Email, cancellationToken);
            if (user == null)
            {
                throw new UnauthorizedAccessException("อีเมลหรือรหัสผ่านไม่ถูกต้อง");
            }

            // In new schema, all users are active by default (no IsActive field)
            // Users are only active if they have completed registration (have password)

            // ตรวจสอบรหัสผ่าน
            var hashedPassword = HashPassword(request.LoginDto.Password);
            if (user.PasswordHash != hashedPassword)
            {
                // บันทึก audit log สำหรับการ login ที่ไม่สำเร็จ
                await _auditLogService.LogLoginAsync(
                    user.Id,
                    request.IpAddress,
                    request.UserAgent,
                    success: false,
                    cancellationToken: cancellationToken);

                throw new UnauthorizedAccessException("อีเมลหรือรหัสผ่านไม่ถูกต้อง");
            }

            // In new schema, we don't track LastLoginAt anymore
            // No need to update user after login

            // สร้าง JWT tokens
            var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user, cancellationToken);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var expiresAt = _jwtTokenService.GetTokenExpiration(accessToken) ?? DateTime.UtcNow.AddHours(1);

            // บันทึก audit log สำหรับการ login ที่สำเร็จ
            await _auditLogService.LogLoginAsync(
                user.Id,
                request.IpAddress,
                request.UserAgent,
                success: true,
                cancellationToken: cancellationToken);

            _logger.LogInformation("User logged in successfully: {UserId}, {Email}", user.Id, user.Email);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                User = _mapper.Map<UserDto>(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.LoginDto.Email);
            throw;
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "salt_key"));
        return Convert.ToBase64String(hashedBytes);
    }
}

// Change Password Command
public record ChangePasswordCommand(int UserId, ChangePasswordDto PasswordDto) : IRequest<bool>;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;
    private readonly IAuditLogService _auditLogService;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        ILogger<ChangePasswordCommandHandler> logger,
        IAuditLogService auditLogService)
    {
        _userRepository = userRepository;
        _logger = logger;
        _auditLogService = auditLogService;
    }

    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException("ไม่พบผู้ใช้งาน");
            }

            // ตรวจสอบรหัสผ่านปัจจุบัน
            var currentHashedPassword = HashPassword(request.PasswordDto.CurrentPassword);
            if (user.PasswordHash != currentHashedPassword)
            {
                throw new UnauthorizedAccessException("รหัสผ่านปัจจุบันไม่ถูกต้อง");
            }

            // อัพเดตรหัสผ่านใหม่
            user.PasswordHash = HashPassword(request.PasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            // บันทึก audit log
            await _auditLogService.LogUpdateAsync(
                request.UserId,
                "User",
                request.UserId,
                new { Action = "PasswordChanged" },
                new { Action = "PasswordChanged", ChangedAt = DateTime.UtcNow },
                cancellationToken: cancellationToken);

            _logger.LogInformation("Password changed successfully for user: {UserId}", request.UserId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user: {UserId}", request.UserId);
            throw;
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "salt_key"));
        return Convert.ToBase64String(hashedBytes);
    }
}