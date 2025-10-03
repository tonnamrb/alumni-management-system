# Authentication Flow Analysis & Recommendations

## 🎯 Current Requirements vs Implementation

### **ข้อกำหนดใหม่ (New Requirements):**
1. **Login Flow**: Login ใช้ `mobilephone + password` เท่านั้น (ไม่ใช้ email)
2. **Data Source**: ข้อมูล user มาจาก backoffice team (import แล้วมี password อยู่แล้ว)
3. **Registration Flow**: Register ต้องตรวจสอบว่าเบอร์มืออยู่ใน database หรือไม่
   - ถ้ามี → ให้ส่ง OTP ได้
   - ถ้าไม่มี → สมัครไม่ได้
4. **OTP System**: ยังไม่มี SMS service จริง → gen เลขเก็บใน DB ก่อน
5. **No Email Registration**: ยังไม่มีการสมัครด้วย email

---

## 📊 Current Implementation Status

### ✅ Already Implemented:
- Mobile phone authentication endpoints
- Mobile phone normalization (Thai format)
- Password hashing with BCrypt
- JWT token generation
- Basic mobile login/register flow

### ❌ Needs Updates:
- Registration flow (currently allows new mobile numbers)
- OTP table and service (currently placeholder)
- Login flow (still references removed fields like `IsActive`, `LastLoginAt`)
- Schema compatibility with new User entity structure

---

## 🛠️ Required Implementation Changes

### 1. Create OTP Management System

#### OTP Entity (New)
```csharp
// src/Domain/Entities/Otp.cs
public class Otp : BaseEntity
{
    public string MobilePhone { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string Purpose { get; set; } = "registration"; // "login", "registration", "password_reset"
    public bool IsUsed { get; set; } = false;
    public DateTime? UsedAt { get; set; }
    public int AttemptCount { get; set; } = 0;
    public int MaxAttempts { get; set; } = 3;
    
    // Domain Methods
    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;
    public bool CanBeUsed() => !IsUsed && !IsExpired() && AttemptCount < MaxAttempts;
    public void MarkAsUsed()
    {
        IsUsed = true;
        UsedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }
}
```

#### Database Migration for OTP Table
```sql
CREATE TABLE "Otps" (
    "Id" SERIAL PRIMARY KEY,
    "MobilePhone" VARCHAR(15) NOT NULL,
    "Code" VARCHAR(6) NOT NULL,
    "ExpiresAt" TIMESTAMP NOT NULL,
    "Purpose" VARCHAR(50) NOT NULL DEFAULT 'registration',
    "IsUsed" BOOLEAN NOT NULL DEFAULT FALSE,
    "UsedAt" TIMESTAMP NULL,
    "AttemptCount" INTEGER NOT NULL DEFAULT 0,
    "MaxAttempts" INTEGER NOT NULL DEFAULT 3,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX "IX_Otps_MobilePhone_Purpose" ON "Otps" ("MobilePhone", "Purpose");
CREATE INDEX "IX_Otps_ExpiresAt" ON "Otps" ("ExpiresAt");
```

### 2. Update Registration Flow

#### Updated Registration Logic
```csharp
// src/Application/Services/AuthenticationService.cs
public async Task<bool> CanRegisterWithMobilePhoneAsync(string mobilePhone)
{
    var normalizedPhone = PhoneNumberHelper.NormalizeMobilePhone(mobilePhone);
    
    // ตรวจสอบว่าเบอร์นี้มีในระบบหรือไม่ (import จาก backoffice)
    var existingUser = await _userRepository.GetByMobilePhoneAsync(normalizedPhone);
    
    // ถ้าไม่มีเบอร์ในระบบ → สมัครไม่ได้
    if (existingUser == null)
    {
        return false;
    }
    
    // ถ้ามีเบอร์แล้ว แต่ยังไม่ได้ตั้ง password → สมัครได้
    return string.IsNullOrEmpty(existingUser.PasswordHash);
}

public async Task<bool> RequestRegistrationOtpAsync(string mobilePhone)
{
    // ตรวจสอบว่าสมัครได้หรือไม่
    if (!await CanRegisterWithMobilePhoneAsync(mobilePhone))
    {
        throw new InvalidOperationException("Mobile phone not found in system or already registered");
    }
    
    var normalizedPhone = PhoneNumberHelper.NormalizeMobilePhone(mobilePhone);
    
    // Generate OTP
    var otpCode = GenerateOtpCode();
    var otp = new Otp
    {
        MobilePhone = normalizedPhone,
        Code = otpCode,
        ExpiresAt = DateTime.UtcNow.AddMinutes(5),
        Purpose = "registration"
    };
    
    await _otpRepository.AddAsync(otp);
    await _otpRepository.SaveChangesAsync();
    
    _logger.LogInformation("Registration OTP generated for phone: {MobilePhone}, Code: {Code}", 
        normalizedPhone, otpCode);
    
    return true;
}

public async Task<bool> VerifyRegistrationOtpAsync(string mobilePhone, string otpCode)
{
    var normalizedPhone = PhoneNumberHelper.NormalizeMobilePhone(mobilePhone);
    
    var otp = await _otpRepository.GetActiveOtpAsync(normalizedPhone, "registration");
    
    if (otp == null || !otp.CanBeUsed())
    {
        return false;
    }
    
    if (otp.Code != otpCode)
    {
        otp.AttemptCount++;
        await _otpRepository.UpdateAsync(otp);
        await _otpRepository.SaveChangesAsync();
        return false;
    }
    
    otp.MarkAsUsed();
    await _otpRepository.UpdateAsync(otp);
    await _otpRepository.SaveChangesAsync();
    
    return true;
}

public async Task<User> CompleteRegistrationAsync(string mobilePhone, string password)
{
    var normalizedPhone = PhoneNumberHelper.NormalizeMobilePhone(mobilePhone);
    var user = await _userRepository.GetByMobilePhoneAsync(normalizedPhone);
    
    if (user == null)
    {
        throw new InvalidOperationException("User not found");
    }
    
    if (!string.IsNullOrEmpty(user.PasswordHash))
    {
        throw new InvalidOperationException("User already registered");
    }
    
    // Set password
    user.PasswordHash = _passwordService.HashPassword(password);
    
    await _userRepository.UpdateAsync(user);
    await _userRepository.SaveChangesAsync();
    
    return user;
}

private string GenerateOtpCode()
{
    var random = new Random();
    return random.Next(100000, 999999).ToString();
}
```

### 3. Update Login Flow

#### Fixed Login Service (Compatible with New Schema)
```csharp
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

        // ตรวจสอบว่ามี password หรือยัง
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

        // Generate JWT token (ไม่ต้องอัพเดท LastLogin เพราะไม่มี field นี้แล้ว)
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
```

### 4. Updated API Endpoints

#### Registration Flow Endpoints
```csharp
// src/Api/Controllers/AuthController.cs

[HttpPost("check-mobile")]
[ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
public async Task<ActionResult<ApiResponseDto<bool>>> CheckMobilePhone([FromBody] CheckMobileRequest request)
{
    try
    {
        var canRegister = await _authService.CanRegisterWithMobilePhoneAsync(request.MobilePhone);
        
        if (canRegister)
        {
            return Ok(SuccessResponse(true, "เบอร์โทรศัพท์สามารถลงทะเบียนได้"));
        }
        else
        {
            return Ok(SuccessResponse(false, "เบอร์โทรศัพท์นี้ไม่สามารถลงทะเบียนได้"));
        }
    }
    catch (Exception ex)
    {
        return BadRequest(ErrorResponse<bool>("เกิดข้อผิดพลาดในการตรวจสอบเบอร์โทรศัพท์"));
    }
}

[HttpPost("request-registration-otp")]
[ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
public async Task<ActionResult<ApiResponseDto<bool>>> RequestRegistrationOtp([FromBody] RequestOtpRequest request)
{
    try
    {
        await _authService.RequestRegistrationOtpAsync(request.MobilePhone);
        return Ok(SuccessResponse(true, "ส่งรหัส OTP เรียบร้อยแล้ว"));
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(ErrorResponse<bool>(ex.Message));
    }
    catch (Exception ex)
    {
        return BadRequest(ErrorResponse<bool>("เกิดข้อผิดพลาดในการส่งรหัส OTP"));
    }
}

[HttpPost("verify-registration-otp")]
[ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
public async Task<ActionResult<ApiResponseDto<bool>>> VerifyRegistrationOtp([FromBody] OtpVerificationRequest request)
{
    try
    {
        var isValid = await _authService.VerifyRegistrationOtpAsync(request.MobilePhone, request.OtpCode);
        
        if (isValid)
        {
            return Ok(SuccessResponse(true, "ยืนยันรหัส OTP สำเร็จ"));
        }
        else
        {
            return BadRequest(ErrorResponse<bool>("รหัส OTP ไม่ถูกต้องหรือหมดอายุ"));
        }
    }
    catch (Exception ex)
    {
        return BadRequest(ErrorResponse<bool>("เกิดข้อผิดพลาดในการยืนยันรหัส OTP"));
    }
}

[HttpPost("complete-registration")]
[ProducesResponseType(typeof(ApiResponseDto<UserDto>), 200)]
public async Task<ActionResult<ApiResponseDto<UserDto>>> CompleteRegistration([FromBody] CompleteRegistrationRequest request)
{
    try
    {
        var user = await _authService.CompleteRegistrationAsync(request.MobilePhone, request.Password);
        var userDto = _mapper.Map<UserDto>(user);
        
        return Ok(SuccessResponse(userDto, "ลงทะเบียนสำเร็จ"));
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(ErrorResponse<UserDto>(ex.Message));
    }
    catch (Exception ex)
    {
        return BadRequest(ErrorResponse<UserDto>("เกิดข้อผิดพลาดในการลงทะเบียน"));
    }
}
```

---

## 🔄 Updated Registration Flow

### **ขั้นตอนการลงทะเบียนใหม่:**

1. **Check Mobile Phone**
   ```
   POST /api/v1/auth/check-mobile
   {
     "mobilePhone": "0812345678"
   }
   ```

2. **Request OTP** (ถ้าเบอร์มีในระบบ)
   ```
   POST /api/v1/auth/request-registration-otp
   {
     "mobilePhone": "0812345678"
   }
   ```

3. **Verify OTP**
   ```
   POST /api/v1/auth/verify-registration-otp
   {
     "mobilePhone": "0812345678",
     "otpCode": "123456"
   }
   ```

4. **Complete Registration**
   ```
   POST /api/v1/auth/complete-registration
   {
     "mobilePhone": "0812345678",
     "password": "newpassword123"
   }
   ```

### **ขั้นตอนการ Login:**
```
POST /api/v1/auth/login/mobile
{
  "mobilePhone": "0812345678",
  "password": "userpassword123"
}
```

---

## 📋 Implementation Priority

1. **Phase 1** (2 hours): Create OTP entity and repository
2. **Phase 2** (3 hours): Update authentication service with new registration flow
3. **Phase 3** (2 hours): Update API controllers and DTOs
4. **Phase 4** (1 hour): Testing and validation

**Total**: 8 hours implementation time

---

## 🎯 Next Steps

1. Create OTP table migration
2. Implement OTP repository and service
3. Update AuthenticationService registration methods
4. Update API endpoints for new flow
5. Test end-to-end registration and login process

This approach ensures:
- ✅ Only users imported from backoffice can register
- ✅ Mobile phone + password authentication
- ✅ OTP verification for registration security
- ✅ No email-based registration
- ✅ Compatible with new User schema structure