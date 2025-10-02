using Application.DTOs;
using Application.DTOs.Auth;
using Application.Helpers;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Service for handling user authentication including mobile phone authentication
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordService _passwordService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IPasswordService passwordService,
        IMapper mapper,
        ILogger<AuthenticationService> logger)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _passwordService = passwordService;
        _mapper = mapper;
        _logger = logger;
    }

    #region Email Authentication (Existing)

    public async Task<AuthResult> LoginWithEmailAsync(string email, string password)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Login attempt with non-existent email: {Email}", email);
                return AuthResult.Failed("Invalid email or password");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login attempt with inactive user: {UserId}", user.Id);
                return AuthResult.Failed("Account is deactivated");
            }

            // TODO: Verify password hash (implement password hashing)
            if (!VerifyPasswordHash(password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid password for user: {UserId}", user.Id);
                return AuthResult.Failed("Invalid email or password");
            }

            // Update last login
            user.UpdateLastLogin();
            await _userRepository.UpdateAsync(user);

            // Generate JWT token
            var token = await _jwtTokenService.GenerateAccessTokenAsync(user);
            var userDto = _mapper.Map<UserDto>(user);

            _logger.LogInformation("Successful email login for user: {UserId}", user.Id);
            return AuthResult.Successful(token, userDto, DateTime.UtcNow.AddHours(1));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email login for email: {Email}", email);
            return AuthResult.Failed("An error occurred during login");
        }
    }

    public async Task<User> RegisterWithEmailAsync(string email, string password, string name)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists");
            }

            // Create new user
            var user = new User
            {
                Email = email,
                Name = name,
                PasswordHash = HashPassword(password),
                Provider = "Local",
                Role = UserRole.Alumni,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            _logger.LogInformation("New user registered with email: {UserId}", createdUser.Id);
            
            return createdUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email registration for email: {Email}", email);
            throw;
        }
    }

    #endregion

    #region Mobile Phone Authentication (New)

    public async Task<AuthResult> LoginWithMobilePhoneAsync(string mobilePhone, string password)
    {
        try
        {
            var normalizedPhone = PhoneNumberHelper.NormalizeMobilePhone(mobilePhone);
            var user = await _userRepository.GetByMobilePhoneAsync(normalizedPhone);

            if (user == null)
            {
                _logger.LogWarning("Login attempt with non-existent mobile phone: {MobilePhone}", normalizedPhone);
                return AuthResult.Failed("User not found with this mobile phone number");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login attempt with inactive user: {UserId}", user.Id);
                return AuthResult.Failed("Account is deactivated");
            }

            // Verify password
            if (string.IsNullOrEmpty(user.PasswordHash) || !_passwordService.VerifyPassword(password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid password attempt for mobile phone: {MobilePhone}", normalizedPhone);
                return AuthResult.Failed("Invalid mobile phone or password");
            }

            // Update last login
            user.UpdateLastLogin();
            await _userRepository.UpdateAsync(user);

            // Generate JWT token
            var token = await _jwtTokenService.GenerateAccessTokenAsync(user);
            var userDto = _mapper.Map<UserDto>(user);

            _logger.LogInformation("Successful mobile login for user: {UserId}", user.Id);
            return AuthResult.Successful(token, userDto, DateTime.UtcNow.AddHours(1));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during mobile login for phone: {MobilePhone}", mobilePhone);
            return AuthResult.Failed("An error occurred during login");
        }
    }

    public async Task<User> RegisterWithMobilePhoneAsync(string mobilePhone, string name, string password, string? email = null)
    {
        try
        {
            var normalizedPhone = PhoneNumberHelper.NormalizeMobilePhone(mobilePhone);

            // Check if user already exists
            var existingUser = await _userRepository.GetByMobilePhoneAsync(normalizedPhone);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this mobile phone already exists");
            }

            // Hash password
            var passwordHash = _passwordService.HashPassword(password);

            // Create new user
            var user = new User
            {
                MobilePhone = normalizedPhone,
                Name = name,
                Email = email ?? $"user_{normalizedPhone}@alumni.local", // กำหนด default email หาก null
                PasswordHash = passwordHash,
                Provider = "Mobile",
                Role = UserRole.Alumni,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            _logger.LogInformation("New user registered with mobile phone: {UserId}", createdUser.Id);

            return createdUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during mobile registration for phone: {MobilePhone}", mobilePhone);
            throw;
        }
    }

    #endregion

    #region External Authentication

    public async Task<AuthResult> LoginWithProviderAsync(string provider, string providerId, string email, string name, string? pictureUrl = null)
    {
        try
        {
            // Try to find existing user by provider
            var user = await _userRepository.GetByProviderAsync(provider, providerId);
            
            if (user == null)
            {
                // Try to find by email if provider user doesn't exist
                user = await _userRepository.GetByEmailAsync(email);
                
                if (user != null)
                {
                    // Link existing account with provider
                    user.Provider = provider;
                    user.ProviderId = providerId;
                    user.PictureUrl = pictureUrl;
                    user.UpdateLastLogin();
                    await _userRepository.UpdateAsync(user);
                }
                else
                {
                    // Create new user from provider
                    user = new User
                    {
                        Email = email,
                        Name = name,
                        Provider = provider,
                        ProviderId = providerId,
                        PictureUrl = pictureUrl,
                        Role = UserRole.Alumni,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        LastLoginAt = DateTime.UtcNow
                    };
                    
                    user = await _userRepository.AddAsync(user);
                    await _userRepository.SaveChangesAsync();
                }
            }
            else
            {
                // Update existing provider user
                user.UpdateLastLogin();
                await _userRepository.UpdateAsync(user);
            }

            if (!user.IsActive)
            {
                return AuthResult.Failed("Account is deactivated");
            }

            // Generate JWT token
            var token = await _jwtTokenService.GenerateAccessTokenAsync(user);
            var userDto = _mapper.Map<UserDto>(user);

            _logger.LogInformation("Successful {Provider} login for user: {UserId}", provider, user.Id);
            return AuthResult.Successful(token, userDto, DateTime.UtcNow.AddHours(1));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during {Provider} login for email: {Email}", provider, email);
            return AuthResult.Failed("An error occurred during login");
        }
    }

    #endregion

    #region OTP Operations (Future Implementation)

    public async Task<bool> RequestOtpAsync(string mobilePhone, string purpose = "login")
    {
        try
        {
            var normalizedPhone = PhoneNumberHelper.NormalizeMobilePhone(mobilePhone);
            
            // TODO: Implement SMS OTP service integration
            // For now, return true for development
            _logger.LogInformation("OTP requested for phone: {MobilePhone}, purpose: {Purpose}", normalizedPhone, purpose);
            
            await Task.Delay(100); // Simulate async operation
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting OTP for phone: {MobilePhone}", mobilePhone);
            return false;
        }
    }

    public async Task<bool> VerifyOtpAsync(string mobilePhone, string otpCode)
    {
        try
        {
            var normalizedPhone = PhoneNumberHelper.NormalizeMobilePhone(mobilePhone);
            
            // TODO: Implement OTP verification logic
            // For now, accept "123456" as valid OTP for development
            await Task.Delay(100); // Simulate async operation
            
            bool isValid = otpCode == "123456";
            _logger.LogInformation("OTP verification for phone: {MobilePhone}, result: {IsValid}", normalizedPhone, isValid);
            
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying OTP for phone: {MobilePhone}", mobilePhone);
            return false;
        }
    }

    #endregion

    #region Token Operations

    public async Task<AuthResult> RefreshTokenAsync(string token)
    {
        try
        {
            // TODO: Implement token refresh logic
            await Task.Delay(100); // Simulate async operation
            return AuthResult.Failed("Token refresh not implemented");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return AuthResult.Failed("An error occurred during token refresh");
        }
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        try
        {
            // TODO: Implement token revocation logic (add to blacklist)
            await Task.Delay(100); // Simulate async operation
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking token");
            return false;
        }
    }

    #endregion

    #region Password Operations

    public async Task<bool> ResetPasswordAsync(string email)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                // Don't reveal if email exists or not
                return true;
            }

            // TODO: Implement password reset email sending
            _logger.LogInformation("Password reset requested for user: {UserId}", user.Id);
            
            await Task.Delay(100); // Simulate async operation
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting password reset for email: {Email}", email);
            return false;
        }
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            if (!VerifyPasswordHash(currentPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Password changed for user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
            return false;
        }
    }

    #endregion

    #region Private Helper Methods

    private static string HashPassword(string password)
    {
        // TODO: Implement proper password hashing with BCrypt or similar
        // This is a placeholder - never use in production!
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }

    private static bool VerifyPasswordHash(string password, string hash)
    {
        // TODO: Implement proper password verification
        // This is a placeholder - never use in production!
        try
        {
            var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hashBytes = Convert.FromBase64String(hash);
            return passwordBytes.SequenceEqual(hashBytes);
        }
        catch
        {
            return false;
        }
    }

    #endregion
}