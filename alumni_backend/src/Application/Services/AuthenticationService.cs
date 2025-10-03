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
    private readonly IOtpService _otpService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IPasswordService passwordService,
        IOtpService otpService,
        IMapper mapper,
        ILogger<AuthenticationService> logger)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _passwordService = passwordService;
        _otpService = otpService;
        _mapper = mapper;
        _logger = logger;
    }

    #region Email Authentication (Deprecated in New Schema)

    public Task<AuthResult> LoginWithEmailAsync(string email, string password)
    {
        // Email authentication is not supported in the new backoffice integration schema
        // Only mobile phone + password authentication is supported
        throw new NotSupportedException("Email authentication is not supported. Use mobile phone authentication.");
    }

    public Task<Domain.Entities.User> RegisterWithEmailAsync(string email, string password, string name)
    {
        // Email registration is not supported in the new backoffice integration schema
        // Only mobile phone registration with OTP verification is supported
        throw new NotSupportedException("Email registration is not supported. Use mobile phone registration with OTP verification.");
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
                return AuthResult.Failed("หมายเลขโทรศัพท์หรือรหัสผ่านไม่ถูกต้อง");
            }

            // ตรวจสอบว่ามี password หรือยัง (สำหรับ user ที่ import จาก backoffice แต่ยังไม่ลงทะเบียน)
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                _logger.LogWarning("Login attempt with unregistered user: {MobilePhone}", normalizedPhone);
                return AuthResult.Failed("กรุณาลงทะเบียนก่อนเข้าสู่ระบบ");
            }

            // Verify password
            if (!_passwordService.VerifyPassword(password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid password attempt for mobile phone: {MobilePhone}", normalizedPhone);
                return AuthResult.Failed("หมายเลขโทรศัพท์หรือรหัสผ่านไม่ถูกต้อง");
            }

            // Generate JWT token (ไม่ต้องอัพเดท LastLogin เพราะไม่มี field นี้แล้วใน new schema)
            var token = await _jwtTokenService.GenerateAccessTokenAsync(user);
            var userDto = _mapper.Map<UserDto>(user);

            _logger.LogInformation("Successful mobile login for user: {UserId}", user.Id);
            return AuthResult.Successful(token, userDto, DateTime.UtcNow.AddHours(1));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during mobile login for phone: {MobilePhone}", mobilePhone);
            return AuthResult.Failed("เกิดข้อผิดพลาดในการเข้าสู่ระบบ");
        }
    }

    #endregion

    #region Registration Flow with OTP (New)

    public async Task<bool> CanRegisterWithMobilePhoneAsync(string mobilePhone)
    {
        try
        {
            var normalizedPhone = PhoneNumberHelper.NormalizeMobilePhone(mobilePhone);
            
            // ตรวจสอบว่าเบอร์นี้มีในระบบหรือไม่
            var existingUser = await _userRepository.GetByMobilePhoneAsync(normalizedPhone);
            
            // ถ้าไม่มีเบอร์ในระบบ → ไม่สามารถสมัครได้ (เฉพาะเบอร์ที่มีอยู่แล้วเท่านั้นที่สมัครได้)
            if (existingUser == null)
            {
                _logger.LogInformation("Mobile phone not found in system, cannot register: {MobilePhone}", normalizedPhone);
                return false;
            }
            
            // ถ้ามีเบอร์แล้ว แต่ยังไม่ได้ตั้ง password → สมัครได้
            var canRegister = string.IsNullOrEmpty(existingUser.PasswordHash);
            _logger.LogInformation("Existing mobile phone registration check: {MobilePhone}, Has Password: {HasPassword}, Can Register: {CanRegister}", 
                normalizedPhone, !string.IsNullOrEmpty(existingUser.PasswordHash), canRegister);
            
            return canRegister;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking registration eligibility for phone: {MobilePhone}", mobilePhone);
            return false;
        }
    }

    public async Task<bool> RequestRegistrationOtpAsync(string mobilePhone)
    {
        try
        {
            // ตรวจสอบว่าสมัครได้หรือไม่
            if (!await CanRegisterWithMobilePhoneAsync(mobilePhone))
            {
                throw new InvalidOperationException("เบอร์โทรศัพท์นี้ไม่สามารถลงทะเบียนได้ หรือได้ลงทะเบียนแล้ว");
            }
            
            // ส่ง OTP ผ่าน OtpService
            var otpCode = await _otpService.GenerateOtpAsync(mobilePhone, "registration");
            
            _logger.LogInformation("Registration OTP requested for phone: {MobilePhone}", mobilePhone);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting registration OTP for phone: {MobilePhone}", mobilePhone);
            throw;
        }
    }

    public async Task<bool> VerifyRegistrationOtpAsync(string mobilePhone, string otpCode)
    {
        try
        {
            var isValid = await _otpService.VerifyOtpAsync(mobilePhone, otpCode, "registration");
            
            if (isValid)
            {
                _logger.LogInformation("Registration OTP verified successfully for phone: {MobilePhone}", mobilePhone);
            }
            else
            {
                _logger.LogWarning("Invalid registration OTP for phone: {MobilePhone}", mobilePhone);
            }
            
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying registration OTP for phone: {MobilePhone}", mobilePhone);
            return false;
        }
    }

    public async Task<Domain.Entities.User> CompleteRegistrationAsync(string mobilePhone, string password)
    {
        try
        {
            var normalizedPhone = PhoneNumberHelper.NormalizeMobilePhone(mobilePhone);
            _logger.LogInformation("CompleteRegistration - Normalized phone: {NormalizedPhone} from input: {InputPhone}", 
                normalizedPhone, mobilePhone);
            
            var user = await _userRepository.GetByMobilePhoneAsync(normalizedPhone);
            
            if (user == null)
            {
                _logger.LogWarning("CompleteRegistration - User not found for normalized phone: {NormalizedPhone}", 
                    normalizedPhone);
                throw new InvalidOperationException("ไม่พบข้อมูลผู้ใช้งาน");
            }
            
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                throw new InvalidOperationException("ผู้ใช้งานได้ลงทะเบียนแล้ว");
            }
            
            // Set password
            user.PasswordHash = _passwordService.HashPassword(password);
            
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
            
            _logger.LogInformation("Registration completed for phone: {MobilePhone}, UserId: {UserId}", 
                normalizedPhone, user.Id);
            
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing registration for phone: {MobilePhone}", mobilePhone);
            throw;
        }
    }

    // Legacy method - kept for compatibility but will be deprecated
    public Task<Domain.Entities.User> RegisterWithMobilePhoneAsync(string mobilePhone, string name, string password, string? email = null)
    {
        // This method is deprecated in favor of the new OTP-based registration flow
        // Use CanRegisterWithMobilePhoneAsync -> RequestRegistrationOtpAsync -> VerifyRegistrationOtpAsync -> CompleteRegistrationAsync
        throw new NotSupportedException("This registration method is deprecated. Use the new OTP-based registration flow.");
    }

    #endregion

    #region External Authentication (Deprecated in New Schema)

    public Task<AuthResult> LoginWithProviderAsync(string provider, string providerId, string email, string name, string? pictureUrl = null)
    {
        // External provider authentication is not supported in the new backoffice integration schema
        // Only mobile phone + password authentication is supported
        throw new NotSupportedException("External provider authentication is not supported. Use mobile phone authentication.");
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