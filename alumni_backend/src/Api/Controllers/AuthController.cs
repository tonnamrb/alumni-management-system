using Application.Commands.Users;
using Application.DTOs;
using Application.DTOs.Auth;
using Application.Interfaces.Services;
using Application.Queries.Users;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Authentication และ User management endpoints - Enhanced with Mobile Phone Authentication
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IAuthenticationService _authService;
    private readonly IValidator<LoginDto> _loginValidator;
    private readonly IValidator<CreateUserDto> _createUserValidator;
    private readonly IValidator<ChangePasswordDto> _changePasswordValidator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IMediator mediator,
        IAuthenticationService authService,
        IValidator<LoginDto> loginValidator,
        IValidator<CreateUserDto> createUserValidator,
        IValidator<ChangePasswordDto> changePasswordValidator,
        ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _authService = authService;
        _loginValidator = loginValidator;
        _createUserValidator = createUserValidator;
        _changePasswordValidator = changePasswordValidator;
        _logger = logger;
    }

    /// <summary>
    /// เข้าสู่ระบบ
    /// </summary>
    /// <param name="loginDto">ข้อมูลสำหรับเข้าสู่ระบบ</param>
    /// <returns>JWT token และข้อมูลผู้ใช้งาน</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponseDto<LoginResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            // Validation
            var validationResult = await _loginValidator.ValidateAsync(loginDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new ValidationErrorDto
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                }).ToList();

                return ValidationErrorResponse("ข้อมูลไม่ถูกต้อง", errors);
            }

            // Execute command
            var command = new LoginCommand(loginDto, GetClientIpAddress(), GetClientUserAgent());
            var result = await _mediator.Send(command);

            return Ok(SuccessResponse(result, "เข้าสู่ระบบสำเร็จ"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return UnauthorizedResponse(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการเข้าสู่ระบบ"));
        }
    }

    /// <summary>
    /// สมัครสมาชิกใหม่
    /// </summary>
    /// <param name="createUserDto">ข้อมูลผู้ใช้งานใหม่</param>
    /// <returns>ข้อมูลผู้ใช้งานที่สร้างใหม่</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponseDto<UserDto>), 201)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> Register([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            // Validation
            var validationResult = await _createUserValidator.ValidateAsync(createUserDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new ValidationErrorDto
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                }).ToList();

                return ValidationErrorResponse("ข้อมูลไม่ถูกต้อง", errors);
            }

            // Execute command
            var command = new CreateUserCommand(createUserDto);
            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetProfile), new { id = result.Id }, SuccessResponse(result, "สมัครสมาชิกสำเร็จ"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ErrorResponse<object>(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการสมัครสมาชิก"));
        }
    }

    /// <summary>
    /// ดูข้อมูลโปรไฟล์ตนเอง
    /// </summary>
    /// <returns>ข้อมูลผู้ใช้งานปัจจุบัน</returns>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponseDto<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> GetProfile()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return UnauthorizedResponse("ไม่พบข้อมูลผู้ใช้งานในระบบ");
            }

            var query = new GetUserByIdQuery(currentUserId.Value);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFoundResponse("ไม่พบข้อมูลผู้ใช้งาน");
            }

            return Ok(SuccessResponse(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการดึงข้อมูลโปรไฟล์"));
        }
    }

    /// <summary>
    /// เปลี่ยนรหัสผ่าน
    /// </summary>
    /// <param name="changePasswordDto">ข้อมูลสำหรับเปลี่ยนรหัสผ่าน</param>
    /// <returns>ผลการเปลี่ยนรหัสผ่าน</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    public async Task<ActionResult<ApiResponseDto<bool>>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return UnauthorizedResponse("ไม่พบข้อมูลผู้ใช้งานในระบบ");
            }

            // Validation
            var validationResult = await _changePasswordValidator.ValidateAsync(changePasswordDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new ValidationErrorDto
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                }).ToList();

                return ValidationErrorResponse("ข้อมูลไม่ถูกต้อง", errors);
            }

            // Execute command
            var command = new ChangePasswordCommand(currentUserId.Value, changePasswordDto);
            var result = await _mediator.Send(command);

            return Ok(SuccessResponse(result, "เปลี่ยนรหัสผ่านสำเร็จ"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return UnauthorizedResponse(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการเปลี่ยนรหัสผ่าน"));
        }
    }

    /// <summary>
    /// ออกจากระบบ
    /// </summary>
    /// <returns>ผลการออกจากระบบ</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
    public async Task<ActionResult<ApiResponseDto<bool>>> Logout()
    {
        // ในการ implement จริงอาจจะเก็บ blacklist ของ tokens ที่ logout แล้ว
        // หรือใช้ refresh token mechanism
        return Ok(SuccessResponse(true, "ออกจากระบบสำเร็จ"));
    }

    #region Mobile Phone Authentication

    /// <summary>
    /// เข้าสู่ระบบด้วยหมายเลขโทรศัพท์มือถือ
    /// </summary>
    /// <param name="request">ข้อมูลสำหรับเข้าสู่ระบบด้วยมือถือ</param>
    /// <returns>JWT token และข้อมูลผู้ใช้งาน</returns>
    [HttpPost("login/mobile")]
    [ProducesResponseType(typeof(ApiResponseDto<AuthResult>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 401)]
    public async Task<ActionResult<ApiResponseDto<AuthResult>>> LoginWithMobile([FromBody] MobileLoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponse<object>("ข้อมูลไม่ถูกต้อง"));
            }

            var result = await _authService.LoginWithMobilePhoneAsync(request.MobilePhone, request.Password);

            if (!result.Success)
            {
                _logger.LogWarning("Failed mobile login attempt for phone: {MobilePhone}", request.MobilePhone);
                return Unauthorized(ErrorResponse<object>(result.Error ?? "เข้าสู่ระบบไม่สำเร็จ"));
            }

            _logger.LogInformation("Successful mobile login for phone: {MobilePhone}", request.MobilePhone);
            return Ok(SuccessResponse(result, "เข้าสู่ระบบสำเร็จ"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during mobile login for phone: {MobilePhone}", request.MobilePhone);
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการเข้าสู่ระบบ"));
        }
    }

    /// <summary>
    /// ลงทะเบียนด้วยหมายเลขโทรศัพท์มือถือ
    /// </summary>
    /// <param name="request">ข้อมูลสำหรับลงทะเบียน</param>
    /// <returns>ข้อมูลผู้ใช้งานที่สร้างขึ้น</returns>
    [HttpPost("register/mobile")]
    [ProducesResponseType(typeof(ApiResponseDto<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> RegisterWithMobile([FromBody] MobileRegisterRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponse<object>("ข้อมูลไม่ถูกต้อง"));
            }

            var user = await _authService.RegisterWithMobilePhoneAsync(request.MobilePhone, request.Name, request.Password, request.Email);
            
            // Map to DTO (you may need to create this mapping)
            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                MobilePhone = user.MobilePhone,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };

            _logger.LogInformation("Successful mobile registration for phone: {MobilePhone}", request.MobilePhone);
            return Ok(SuccessResponse(userDto, "ลงทะเบียนสำเร็จ"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Mobile registration failed: {Error}", ex.Message);
            return BadRequest(ErrorResponse<object>(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during mobile registration for phone: {MobilePhone}", request.MobilePhone);
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการลงทะเบียน"));
        }
    }

    /// <summary>
    /// ขอรหัส OTP สำหรับการเข้าสู่ระบบ
    /// </summary>
    /// <param name="request">ข้อมูลการขอ OTP</param>
    /// <returns>ผลการส่ง OTP</returns>
    [HttpPost("request-otp")]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    public async Task<ActionResult<ApiResponseDto<bool>>> RequestOtp([FromBody] RequestOtpRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponse<object>("ข้อมูลไม่ถูกต้อง"));
            }

            var result = await _authService.RequestOtpAsync(request.MobilePhone, request.Purpose);
            
            if (result)
            {
                _logger.LogInformation("OTP requested for phone: {MobilePhone}, purpose: {Purpose}", request.MobilePhone, request.Purpose);
                return Ok(SuccessResponse(true, "ส่งรหัส OTP เรียบร้อยแล้ว"));
            }
            else
            {
                _logger.LogWarning("Failed to request OTP for phone: {MobilePhone}", request.MobilePhone);
                return BadRequest(ErrorResponse<object>("ไม่สามารถส่งรหัส OTP ได้"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting OTP for phone: {MobilePhone}", request.MobilePhone);
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการส่งรหัส OTP"));
        }
    }

    /// <summary>
    /// ตรวจสอบรหัส OTP
    /// </summary>
    /// <param name="request">ข้อมูลการตรวจสอบ OTP</param>
    /// <returns>ผลการตรวจสอบ OTP</returns>
    [HttpPost("verify-otp")]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    public async Task<ActionResult<ApiResponseDto<bool>>> VerifyOtp([FromBody] OtpVerificationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponse<object>("ข้อมูลไม่ถูกต้อง"));
            }

            var isValid = await _authService.VerifyOtpAsync(request.MobilePhone, request.OtpCode);
            
            if (isValid)
            {
                _logger.LogInformation("OTP verified successfully for phone: {MobilePhone}", request.MobilePhone);
                return Ok(SuccessResponse(true, "ตรวจสอบรหัส OTP สำเร็จ"));
            }
            else
            {
                _logger.LogWarning("Invalid OTP for phone: {MobilePhone}", request.MobilePhone);
                return BadRequest(ErrorResponse<object>("รหัส OTP ไม่ถูกต้อง"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying OTP for phone: {MobilePhone}", request.MobilePhone);
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการตรวจสอบรหัส OTP"));
        }
    }

    #endregion
}