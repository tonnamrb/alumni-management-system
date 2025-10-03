using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Application.Commands.Users;

// Create User Command
public record CreateUserCommand(CreateUserDto UserDto) : IRequest<UserDto>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateUserCommandHandler> _logger;
    private readonly IAuditLogService _auditLogService;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<CreateUserCommandHandler> logger,
        IAuditLogService auditLogService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _auditLogService = auditLogService;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // ตรวจสอบว่าอีเมลมีอยู่แล้วหรือไม่
            var existingUser = await _userRepository.GetByEmailAsync(request.UserDto.Email, cancellationToken);
            if (existingUser != null)
            {
                throw new InvalidOperationException("อีเมลนี้ถูกใช้งานแล้ว");
            }

            // สร้าง User entity
            var user = _mapper.Map<User>(request.UserDto);
            
            // Hash password หากไม่ใช่ OAuth
            if (!string.IsNullOrEmpty(request.UserDto.Password))
            {
                user.PasswordHash = HashPassword(request.UserDto.Password);
            }

            // บันทึกใน database
            var savedUser = await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            // บันทึก audit log
            await _auditLogService.LogCreateAsync(
                savedUser.Id,
                "User",
                savedUser.Id,
                savedUser,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Created new user with ID: {UserId}, Email: {Email}", savedUser.Id, savedUser.Email);

            return _mapper.Map<UserDto>(savedUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with email: {Email}", request.UserDto.Email);
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

// Update User Command
public record UpdateUserCommand(int UserId, UpdateUserDto UserDto) : IRequest<UserDto>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateUserCommandHandler> _logger;
    private readonly IAuditLogService _auditLogService;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<UpdateUserCommandHandler> logger,
        IAuditLogService auditLogService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _auditLogService = auditLogService;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingUser = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (existingUser == null)
            {
                throw new InvalidOperationException("ไม่พบผู้ใช้งาน");
            }

            // ตรวจสอบว่าอีเมลใหม่ถูกใช้งานโดยคนอื่นหรือไม่
            if (existingUser.Email != request.UserDto.Email)
            {
                var emailExists = await _userRepository.IsEmailExistsAsync(request.UserDto.Email, request.UserId, cancellationToken);
                if (emailExists)
                {
                    throw new InvalidOperationException("อีเมลนี้ถูกใช้งานแล้ว");
                }
            }

            // เก็บข้อมูลเดิมสำหรับ audit log
            var oldUserData = new
            {
                existingUser.FullName,
                existingUser.Email
            };

            // อัพเดตข้อมูล
            _mapper.Map(request.UserDto, existingUser);
            
            var updatedUser = await _userRepository.UpdateAsync(existingUser, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            // บันทึก audit log
            await _auditLogService.LogUpdateAsync(
                request.UserId,
                "User",
                request.UserId,
                oldUserData,
                new { updatedUser.FullName, updatedUser.Email },
                cancellationToken: cancellationToken);

            _logger.LogInformation("Updated user with ID: {UserId}", request.UserId);

            return _mapper.Map<UserDto>(updatedUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID: {UserId}", request.UserId);
            throw;
        }
    }
}

// Delete User Command
public record DeleteUserCommand(int UserId) : IRequest<bool>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<DeleteUserCommandHandler> _logger;
    private readonly IAuditLogService _auditLogService;

    public DeleteUserCommandHandler(
        IUserRepository userRepository,
        ILogger<DeleteUserCommandHandler> logger,
        IAuditLogService auditLogService)
    {
        _userRepository = userRepository;
        _logger = logger;
        _auditLogService = auditLogService;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return false;
            }

            // เก็บข้อมูลสำหรับ audit log
            var userData = new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.RoleId
            };

            await _userRepository.DeleteAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            // บันทึก audit log
            await _auditLogService.LogDeleteAsync(
                request.UserId,
                "User",
                request.UserId,
                userData,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Deleted user with ID: {UserId}", request.UserId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID: {UserId}", request.UserId);
            throw;
        }
    }
}