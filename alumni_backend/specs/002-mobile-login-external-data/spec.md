# Alumni Backend API - Mobile Login & External Data Integration

## Project Enhancement Specification v2.0

### Overview
การปรับปรุง Alumni Backend API เพื่อรองรับการ login ด้วยหมายเลขโทรศัพท์มือถือและการรับข้อมูล alumni จากระบบภายนอก

### Changes Summary
- **Login System**: เปลี่ยนจาก Email-based เป็น MobilePhone-based authentication
- **Entity Structure**: ปรับปรุงเพื่อรองรับข้อมูลจากระบบภายนอกแบบยืดหยุ่น
- **External Data Integration**: เพิ่ม API endpoints สำหรับรับข้อมูลจากระบบภายนอก
- **Data Flexibility**: รองรับข้อมูลแบบ string-based สำหรับการ integrate กับระบบต่างๆ

---

## Task 2.4: Enhanced Entity Structure for External Data Integration (6 hours)

### Objectives
- ปรับปรุง User และ AlumniProfile entities เพื่อรองรับ MobilePhone authentication
- เพิ่มความยืดหยุ่นในการจัดเก็บข้อมูลจากระบบภายนอก
- รองรับการติดตาม external data sources

### User Entity Enhancements

```csharp
public class User : BaseEntity
{
    // Core Authentication Fields
    public string MobilePhone { get; set; } = string.Empty;  // Primary login field
    public string? Email { get; set; }  // Optional secondary contact
    public string Name { get; set; } = string.Empty;
    
    // External System Integration
    public string? ExternalMemberID { get; set; }  // รหัสสมาชิกจากระบบเดิม
    public string? ExternalSystemId { get; set; }  // ระบุแหล่งที่มาของข้อมูล
    public DateTime? ExternalDataLastSync { get; set; }  // วันที่ sync ล่าสุด
    
    // Authentication & Security
    public string? Provider { get; set; } // "Mobile", "Google", "Facebook", "External"
    public string? ProviderId { get; set; }
    public string? PictureUrl { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation
    public AlumniProfile? AlumniProfile { get; set; }
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
```

### AlumniProfile Entity Enhancements

```csharp
public class AlumniProfile : BaseEntity
{
    // Foreign Key
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    // External Data Tracking
    public string? ExternalMemberID { get; set; }  // รหัสสมาชิกจากระบบเดิม
    public string? ExternalSystemId { get; set; }  // แหล่งที่มาของข้อมูล
    public DateTime? ExternalDataLastSync { get; set; }
    
    // Personal Information (Flexible String Fields)
    public string? NameInYearbook { get; set; }  // ชื่อในรุ่น
    public string? TitleCode { get; set; }  // "Mr.", "Mrs.", "Dr." etc. (แทนที่ TitleID)
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? NickName { get; set; }
    
    // Group & Classification
    public string? GroupCode { get; set; }  // รหัสกลุ่ม/สาขา (แทนที่ GroupID)
    public string? ClassName { get; set; }  // ชั้นปี/รุ่น
    public int? GraduationYear { get; set; }
    
    // Contact Information
    public string? Phone { get; set; }  // โทรศัพท์บ้าน/ที่ทำงาน
    public string? MobilePhone { get; set; }  // โทรศัพท์มือถือ (อาจซ้ำกับ User.MobilePhone)
    public string? LineID { get; set; }
    public string? Facebook { get; set; }
    public string? Email { get; set; }  // อาจซ้ำกับ User.Email
    
    // Address Information
    public string? Address { get; set; }  // ที่อยู่เต็ม
    public string? ZipCode { get; set; }
    public string? District { get; set; }  // ตำบล/แขวง
    public string? Province { get; set; }  // จังหวัด
    public string? Country { get; set; } = "Thailand";
    
    // Professional Information
    public string? CompanyName { get; set; }
    public string? JobTitle { get; set; }
    public string? WorkAddress { get; set; }
    
    // Personal Status
    public string? MaritalStatus { get; set; }  // "Single", "Married", "Divorced" etc.
    public string? SpouseName { get; set; }
    public string? Status { get; set; }  // สถานะสมาชิก
    
    // Additional Information
    public string? Comment { get; set; }  // หมายเหตุ
    public string? Bio { get; set; }  // ประวัติส่วนตัว
    public DateTime? DateOfBirth { get; set; }
    
    // Metadata
    public bool IsVerified { get; set; } = false;  // ยืนยันข้อมูลแล้ว
    public DateTime? ProfileCompletedAt { get; set; }
}
```

### Database Migration Tasks
1. เพิ่ม columns ใหม่ในตาราง Users และ AlumniProfiles
2. สร้าง indexes สำหรับ MobilePhone, ExternalMemberID
3. เพิ่ม unique constraints และ validation rules
4. ทำ data migration สำหรับข้อมูลเดิม (ถ้ามี)

---

## Task 4.7: Mobile Phone Authentication System (4 hours)

### Objectives
- เปลี่ยน primary authentication จาก Email เป็น MobilePhone
- รองรับรูปแบบเบอร์โทรศัพท์ไทย
- เพิ่ม phone number normalization และ validation

### Phone Number Normalization

```csharp
public static class PhoneNumberHelper
{
    public static string NormalizeMobilePhone(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return string.Empty;
        
        // Remove all non-digits
        string digits = Regex.Replace(phoneNumber, @"[^\d]", "");
        
        // Handle Thai mobile number formats
        if (digits.StartsWith("66")) // +66 format
        {
            digits = "0" + digits.Substring(2);
        }
        else if (digits.Length == 9 && !digits.StartsWith("0")) // 9-digit without leading 0
        {
            digits = "0" + digits;
        }
        
        // Validate Thai mobile number format (0x-xxxx-xxxx)
        if (digits.Length == 10 && digits.StartsWith("0") && 
            (digits.StartsWith("06") || digits.StartsWith("08") || digits.StartsWith("09")))
        {
            return digits;
        }
        
        throw new ArgumentException("Invalid Thai mobile phone number format");
    }
    
    public static bool IsValidThaiMobilePhone(string phoneNumber)
    {
        try
        {
            NormalizeMobilePhone(phoneNumber);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
```

### Authentication Service Updates

```csharp
public class AuthenticationService : IAuthenticationService
{
    public async Task<AuthResult> LoginWithMobilePhoneAsync(string mobilePhone, string? otp = null)
    {
        var normalizedPhone = PhoneNumberHelper.NormalizeMobilePhone(mobilePhone);
        var user = await _userRepository.GetByMobilePhoneAsync(normalizedPhone);
        
        if (user == null)
        {
            return AuthResult.Failed("User not found");
        }
        
        // TODO: Implement OTP verification
        // For now, direct login for development
        
        var token = GenerateJwtToken(user);
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
        
        return AuthResult.Success(token, user);
    }
    
    public async Task<User> RegisterWithMobilePhoneAsync(string mobilePhone, string name)
    {
        var normalizedPhone = PhoneNumberHelper.NormalizeMobilePhone(mobilePhone);
        
        // Check if user already exists
        var existingUser = await _userRepository.GetByMobilePhoneAsync(normalizedPhone);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this mobile phone already exists");
        }
        
        var user = new User
        {
            MobilePhone = normalizedPhone,
            Name = name,
            Provider = "Mobile",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        return await _userRepository.CreateAsync(user);
    }
}
```

### API Controller Updates

```csharp
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("login/mobile")]
    public async Task<IActionResult> LoginWithMobile([FromBody] MobileLoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, error = "Invalid request" });
            
        var result = await _authService.LoginWithMobilePhoneAsync(request.MobilePhone, request.Otp);
        
        if (!result.Success)
            return Unauthorized(new { success = false, error = result.Error });
            
        return Ok(new 
        { 
            success = true, 
            data = new { token = result.Token, user = result.User },
            error = (string?)null 
        });
    }
    
    [HttpPost("register/mobile")]
    public async Task<IActionResult> RegisterWithMobile([FromBody] MobileRegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, error = "Invalid request" });
            
        try
        {
            var user = await _authService.RegisterWithMobilePhoneAsync(request.MobilePhone, request.Name);
            return Ok(new { success = true, data = user, error = (string?)null });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }
}
```

---

## Task 4.10: External Data Integration Service (10 hours)

### Objectives
- สร้างระบบรับและประมวลผลข้อมูล alumni จากระบบภายนอก
- รองรับการ import ข้อมูลแบบ bulk
- มีระบบ validation และ error handling
- สามารถติดตามแหล่งที่มาของข้อมูล

### External Data DTOs

```csharp
public class ExternalAlumniData
{
    public string MemberID { get; set; } = string.Empty;
    public string? NameInYearbook { get; set; }
    public string? TitleID { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? NickName { get; set; }
    public string? GroupID { get; set; }
    public string? Phone { get; set; }
    public string? MobilePhone { get; set; }
    public string? LineID { get; set; }
    public string? Facebook { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? ZipCode { get; set; }
    public string? CompanyName { get; set; }
    public string? Status { get; set; }
    public string? SpouseName { get; set; }
    public string? Comment { get; set; }
}

public class BulkImportRequest
{
    public string ExternalSystemId { get; set; } = string.Empty;
    public List<ExternalAlumniData> Alumni { get; set; } = new();
    public bool OverwriteExisting { get; set; } = false;
    public bool ValidateOnly { get; set; } = false;
}

public class ImportResult
{
    public int TotalRecords { get; set; }
    public int SuccessfulImports { get; set; }
    public int FailedImports { get; set; }
    public int SkippedRecords { get; set; }
    public List<ImportError> Errors { get; set; } = new();
    public DateTime ProcessedAt { get; set; }
}

public class ImportError
{
    public string MemberID { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public string? ReceivedValue { get; set; }
}
```

### External Data Integration Service

```csharp
public interface IExternalDataIntegrationService
{
    Task<ImportResult> ProcessBulkDataAsync(BulkImportRequest request);
    Task<ImportResult> ValidateDataAsync(List<ExternalAlumniData> data, string externalSystemId);
    Task<bool> SyncSingleRecordAsync(ExternalAlumniData data, string externalSystemId);
}

public class ExternalDataIntegrationService : IExternalDataIntegrationService
{
    private readonly IUserRepository _userRepository;
    private readonly IAlumniProfileRepository _profileRepository;
    private readonly ILogger<ExternalDataIntegrationService> _logger;

    public async Task<ImportResult> ProcessBulkDataAsync(BulkImportRequest request)
    {
        var result = new ImportResult
        {
            TotalRecords = request.Alumni.Count,
            ProcessedAt = DateTime.UtcNow
        };

        foreach (var alumniData in request.Alumni)
        {
            try
            {
                if (request.ValidateOnly)
                {
                    ValidateAlumniData(alumniData, result);
                }
                else
                {
                    await ProcessSingleRecordAsync(alumniData, request.ExternalSystemId, request.OverwriteExisting, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing alumni record {MemberID}", alumniData.MemberID);
                result.Errors.Add(new ImportError
                {
                    MemberID = alumniData.MemberID,
                    Field = "General",
                    Error = ex.Message
                });
                result.FailedImports++;
            }
        }

        return result;
    }

    private async Task ProcessSingleRecordAsync(ExternalAlumniData data, string externalSystemId, bool overwriteExisting, ImportResult result)
    {
        // Normalize and validate mobile phone
        string? normalizedMobile = null;
        if (!string.IsNullOrWhiteSpace(data.MobilePhone))
        {
            try
            {
                normalizedMobile = PhoneNumberHelper.NormalizeMobilePhone(data.MobilePhone);
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ImportError
                {
                    MemberID = data.MemberID,
                    Field = "MobilePhone",
                    Error = ex.Message,
                    ReceivedValue = data.MobilePhone
                });
            }
        }

        // Find existing user by external member ID or mobile phone
        var existingUser = await FindExistingUserAsync(data.MemberID, normalizedMobile);

        if (existingUser != null && !overwriteExisting)
        {
            result.SkippedRecords++;
            return;
        }

        if (existingUser == null)
        {
            // Create new user and profile
            await CreateNewUserWithProfileAsync(data, externalSystemId, normalizedMobile);
            result.SuccessfulImports++;
        }
        else
        {
            // Update existing user and profile
            await UpdateExistingUserWithProfileAsync(existingUser, data, externalSystemId);
            result.SuccessfulImports++;
        }
    }

    private async Task<User?> FindExistingUserAsync(string externalMemberID, string? normalizedMobile)
    {
        // Try to find by external member ID first
        var user = await _userRepository.GetByExternalMemberIDAsync(externalMemberID);
        
        if (user == null && !string.IsNullOrWhiteSpace(normalizedMobile))
        {
            // Try to find by mobile phone
            user = await _userRepository.GetByMobilePhoneAsync(normalizedMobile);
        }

        return user;
    }

    private async Task CreateNewUserWithProfileAsync(ExternalAlumniData data, string externalSystemId, string? normalizedMobile)
    {
        var user = new User
        {
            MobilePhone = normalizedMobile ?? string.Empty,
            Email = data.Email,
            Name = $"{data.Firstname} {data.Lastname}".Trim(),
            ExternalMemberID = data.MemberID,
            ExternalSystemId = externalSystemId,
            ExternalDataLastSync = DateTime.UtcNow,
            Provider = "External",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.CreateAsync(user);

        var profile = new AlumniProfile
        {
            UserId = createdUser.Id,
            ExternalMemberID = data.MemberID,
            ExternalSystemId = externalSystemId,
            ExternalDataLastSync = DateTime.UtcNow,
            NameInYearbook = data.NameInYearbook,
            TitleCode = data.TitleID,
            Firstname = data.Firstname,
            Lastname = data.Lastname,
            NickName = data.NickName,
            GroupCode = data.GroupID,
            Phone = data.Phone,
            MobilePhone = data.MobilePhone,
            LineID = data.LineID,
            Facebook = data.Facebook,
            Email = data.Email,
            Address = data.Address,
            ZipCode = data.ZipCode,
            CompanyName = data.CompanyName,
            Status = data.Status,
            SpouseName = data.SpouseName,
            Comment = data.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _profileRepository.CreateAsync(profile);
    }

    private void ValidateAlumniData(ExternalAlumniData data, ImportResult result)
    {
        if (string.IsNullOrWhiteSpace(data.MemberID))
        {
            result.Errors.Add(new ImportError
            {
                MemberID = data.MemberID,
                Field = "MemberID",
                Error = "MemberID is required"
            });
        }

        if (!string.IsNullOrWhiteSpace(data.MobilePhone) && 
            !PhoneNumberHelper.IsValidThaiMobilePhone(data.MobilePhone))
        {
            result.Errors.Add(new ImportError
            {
                MemberID = data.MemberID,
                Field = "MobilePhone",
                Error = "Invalid Thai mobile phone format",
                ReceivedValue = data.MobilePhone
            });
        }

        // Additional validation rules...
    }
}
```

---

## Task 5.8: External Data API Controllers (6 hours)

### Objectives
- สร้าง API endpoints สำหรับรับข้อมูลจากระบบภายนอก
- รองรับ bulk import และ real-time sync
- มีระบบ authentication และ authorization
- รองรับ webhook สำหรับ real-time updates

### External Data Controller

```csharp
[ApiController]
[Route("api/v1/external-data")]
[Authorize(Roles = "Admin,DataManager")]
public class ExternalDataController : ControllerBase
{
    private readonly IExternalDataIntegrationService _integrationService;
    private readonly ILogger<ExternalDataController> _logger;

    public ExternalDataController(
        IExternalDataIntegrationService integrationService,
        ILogger<ExternalDataController> logger)
    {
        _integrationService = integrationService;
        _logger = logger;
    }

    [HttpPost("bulk-import")]
    public async Task<IActionResult> BulkImport([FromBody] BulkImportRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, error = "Invalid request data" });

        try
        {
            _logger.LogInformation("Starting bulk import for {Count} records from system {SystemId}", 
                request.Alumni.Count, request.ExternalSystemId);

            var result = await _integrationService.ProcessBulkDataAsync(request);

            return Ok(new 
            { 
                success = true, 
                data = result, 
                error = (string?)null 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bulk import failed for system {SystemId}", request.ExternalSystemId);
            return StatusCode(500, new 
            { 
                success = false, 
                error = "Internal server error during bulk import" 
            });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateData([FromBody] BulkImportRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, error = "Invalid request data" });

        try
        {
            var result = await _integrationService.ValidateDataAsync(request.Alumni, request.ExternalSystemId);
            
            return Ok(new 
            { 
                success = true, 
                data = result, 
                error = (string?)null 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Data validation failed for system {SystemId}", request.ExternalSystemId);
            return StatusCode(500, new 
            { 
                success = false, 
                error = "Internal server error during validation" 
            });
        }
    }

    [HttpPost("sync-single")]
    public async Task<IActionResult> SyncSingleRecord([FromBody] SyncSingleRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, error = "Invalid request data" });

        try
        {
            var success = await _integrationService.SyncSingleRecordAsync(request.AlumniData, request.ExternalSystemId);
            
            return Ok(new 
            { 
                success = true, 
                data = new { synced = success }, 
                error = (string?)null 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Single record sync failed for member {MemberID}", request.AlumniData.MemberID);
            return StatusCode(500, new 
            { 
                success = false, 
                error = "Internal server error during sync" 
            });
        }
    }

    [HttpPost("webhook/{systemId}")]
    [AllowAnonymous] // ใช้ API key authentication แทน
    public async Task<IActionResult> HandleWebhook(string systemId, [FromBody] WebhookPayload payload)
    {
        // TODO: Implement webhook signature validation
        
        try
        {
            _logger.LogInformation("Received webhook from system {SystemId} with {Count} records", 
                systemId, payload.Records.Count);

            var request = new BulkImportRequest
            {
                ExternalSystemId = systemId,
                Alumni = payload.Records,
                OverwriteExisting = payload.OverwriteExisting
            };

            var result = await _integrationService.ProcessBulkDataAsync(request);
            
            return Ok(new 
            { 
                success = true, 
                data = result, 
                error = (string?)null 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook processing failed for system {SystemId}", systemId);
            return StatusCode(500, new 
            { 
                success = false, 
                error = "Webhook processing failed" 
            });
        }
    }
}

public class SyncSingleRequest
{
    public string ExternalSystemId { get; set; } = string.Empty;
    public ExternalAlumniData AlumniData { get; set; } = new();
}

public class WebhookPayload
{
    public List<ExternalAlumniData> Records { get; set; } = new();
    public bool OverwriteExisting { get; set; } = false;
    public string EventType { get; set; } = "bulk_update";
    public DateTime Timestamp { get; set; }
}
```

### Data Management Controller

```csharp
[ApiController]
[Route("api/v1/data-management")]
[Authorize(Roles = "Admin")]
public class DataManagementController : ControllerBase
{
    [HttpGet("import-history")]
    public async Task<IActionResult> GetImportHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        // TODO: Implement import history tracking
        return Ok(new { success = true, data = new { imports = new List<object>() }, error = (string?)null });
    }

    [HttpGet("external-systems")]
    public async Task<IActionResult> GetExternalSystems()
    {
        // TODO: Implement external systems registry
        return Ok(new { success = true, data = new { systems = new List<object>() }, error = (string?)null });
    }

    [HttpPost("cleanup-duplicates")]
    public async Task<IActionResult> CleanupDuplicates()
    {
        // TODO: Implement duplicate cleanup logic
        return Ok(new { success = true, data = new { cleaned = 0 }, error = (string?)null });
    }

    [HttpGet("data-quality-report")]
    public async Task<IActionResult> GetDataQualityReport()
    {
        // TODO: Implement data quality analysis
        return Ok(new { success = true, data = new { report = new object() }, error = (string?)null });
    }
}
```

---

## Implementation Plan

### Phase 1: Entity Structure & Database Setup (Day 1 - 6 hours)
**Task 2.4: Enhanced Entity Structure for External Data Integration**

#### Step 1.1: Update Domain Entities (2 hours)
- [ ] Backup existing User.cs และ AlumniProfile.cs entities
- [ ] Add new properties to User entity:
  - MobilePhone (string, required)
  - ExternalMemberID (string?, nullable)
  - ExternalSystemId (string?, nullable)  
  - ExternalDataLastSync (DateTime?, nullable)
  - IsActive (bool, default true)
- [ ] Add new properties to AlumniProfile entity:
  - ExternalMemberID, ExternalSystemId, ExternalDataLastSync
  - NameInYearbook, TitleCode, GroupCode (string fields)
  - District, Province, Country, JobTitle, WorkAddress
  - MaritalStatus, Bio, DateOfBirth, IsVerified, ProfileCompletedAt
- [ ] Update entity relationships and navigation properties

#### Step 1.2: Create Database Migrations (2 hours)
- [ ] Run: `dotnet ef migrations add AddMobileLoginAndExternalData -p Infrastructure -s Api`
- [ ] Review generated migration files
- [ ] Add custom indexes for performance:
  ```sql
  CREATE UNIQUE INDEX IX_Users_MobilePhone ON Users (MobilePhone);
  CREATE INDEX IX_Users_ExternalMemberID ON Users (ExternalMemberID);
  CREATE INDEX IX_AlumniProfiles_ExternalMemberID ON AlumniProfiles (ExternalMemberID);
  ```
- [ ] Apply migration: `dotnet ef database update -p Infrastructure -s Api`

#### Step 1.3: Update Repository Interfaces (1 hour)
- [ ] Add methods to IUserRepository:
  - `Task<User?> GetByMobilePhoneAsync(string mobilePhone)`
  - `Task<User?> GetByExternalMemberIDAsync(string externalMemberID)`
  - `Task<List<User>> GetByExternalSystemIdAsync(string systemId)`
- [ ] Add methods to IAlumniProfileRepository:
  - `Task<AlumniProfile?> GetByExternalMemberIDAsync(string externalMemberID)`
  - `Task<List<AlumniProfile>> GetUnverifiedProfilesAsync()`

#### Step 1.4: Implement Repository Methods (1 hour)
- [ ] Update UserRepository with new query methods
- [ ] Update AlumniProfileRepository with new query methods
- [ ] Test repository methods with unit tests

### Phase 2: Mobile Authentication System (Day 2 - 4 hours)
**Task 4.7: Mobile Phone Authentication System**

#### Step 2.1: Create Phone Number Helper (1 hour)
- [ ] Create `Core/Helpers/PhoneNumberHelper.cs`
- [ ] Implement `NormalizeMobilePhone()` method
- [ ] Implement `IsValidThaiMobilePhone()` method
- [ ] Write unit tests for phone number validation

#### Step 2.2: Update Authentication DTOs (0.5 hours)
- [ ] Create `Application/DTOs/Auth/MobileLoginRequest.cs`
- [ ] Create `Application/DTOs/Auth/MobileRegisterRequest.cs`
- [ ] Add validation attributes for phone numbers

#### Step 2.3: Update Authentication Service (1.5 hours)
- [ ] Add methods to IAuthenticationService:
  - `Task<AuthResult> LoginWithMobilePhoneAsync(string mobilePhone, string? otp)`
  - `Task<User> RegisterWithMobilePhoneAsync(string mobilePhone, string name)`
- [ ] Implement methods in AuthenticationService
- [ ] Update JWT token generation to include mobile phone
- [ ] Add phone number normalization in registration

#### Step 2.4: Update Auth Controller (1 hour)
- [ ] Add `[HttpPost("login/mobile")]` endpoint
- [ ] Add `[HttpPost("register/mobile")]` endpoint
- [ ] Update existing endpoints to support mobile phone fallback
- [ ] Test mobile authentication endpoints

### Phase 3: External Data Integration Service (Day 3-4 - 10 hours)
**Task 4.10: External Data Integration Service**

#### Step 3.1: Create External Data DTOs (1 hour)
- [ ] Create `Application/DTOs/ExternalData/ExternalAlumniData.cs`
- [ ] Create `Application/DTOs/ExternalData/BulkImportRequest.cs`
- [ ] Create `Application/DTOs/ExternalData/ImportResult.cs`
- [ ] Create `Application/DTOs/ExternalData/ImportError.cs`
- [ ] Add validation attributes

#### Step 3.2: Create Integration Service Interface (1 hour)
- [ ] Create `Application/Interfaces/IExternalDataIntegrationService.cs`
- [ ] Define service methods:
  - `ProcessBulkDataAsync()`
  - `ValidateDataAsync()`
  - `SyncSingleRecordAsync()`

#### Step 3.3: Implement Integration Service (6 hours)
- [ ] Create `Application/Services/ExternalDataIntegrationService.cs`
- [ ] Implement bulk data processing logic
- [ ] Add phone number normalization for external data
- [ ] Implement user/profile creation and updating
- [ ] Add comprehensive error handling and logging
- [ ] Implement data validation rules

#### Step 3.4: Add Service Registration (0.5 hours)
- [ ] Register service in `Application/DependencyInjection.cs`
- [ ] Add logging configuration

#### Step 3.5: Create Unit Tests (1.5 hours)
- [ ] Test bulk import scenarios
- [ ] Test data validation logic
- [ ] Test error handling cases
- [ ] Test phone number normalization

### Phase 4: External Data API Controllers (Day 5 - 6 hours)
**Task 5.8: External Data API Controllers**

#### Step 4.1: Create External Data Controller (3 hours)
- [ ] Create `Api/Controllers/ExternalDataController.cs`
- [ ] Implement endpoints:
  - `POST /api/v1/external-data/bulk-import`
  - `POST /api/v1/external-data/validate`
  - `POST /api/v1/external-data/sync-single`
  - `POST /api/v1/external-data/webhook/{systemId}`
- [ ] Add authorization and validation
- [ ] Add comprehensive error handling

#### Step 4.2: Create Data Management Controller (2 hours)
- [ ] Create `Api/Controllers/DataManagementController.cs`
- [ ] Implement admin endpoints:
  - `GET /api/v1/data-management/import-history`
  - `GET /api/v1/data-management/external-systems`
  - `POST /api/v1/data-management/cleanup-duplicates`
  - `GET /api/v1/data-management/data-quality-report`

#### Step 4.3: Update API Documentation (1 hour)
- [ ] Add Swagger annotations for new endpoints
- [ ] Update API documentation
- [ ] Test all endpoints with Postman/Swagger

### Phase 5: Integration Testing & Quality Assurance (Day 6 - 4 hours)

#### Step 5.1: Integration Testing (2 hours)
- [ ] Test complete mobile authentication flow
- [ ] Test bulk data import with sample CSV data
- [ ] Test webhook integration
- [ ] Test error scenarios and edge cases

#### Step 5.2: Performance Testing (1 hour)
- [ ] Test bulk import with 1000+ records
- [ ] Monitor database performance
- [ ] Test concurrent API requests
- [ ] Optimize queries if needed

#### Step 5.3: Security & Final Validation (1 hour)
- [ ] Validate authentication and authorization
- [ ] Test input validation and sanitization
- [ ] Review audit logging
- [ ] Final API testing and documentation review

### Daily Implementation Schedule

#### Day 1 (6 hours): Foundation
```
09:00-11:00  Step 1.1: Update Domain Entities
11:15-13:15  Step 1.2: Create Database Migrations  
14:00-15:00  Step 1.3: Update Repository Interfaces
15:15-16:15  Step 1.4: Implement Repository Methods
```

#### Day 2 (4 hours): Authentication
```
09:00-10:00  Step 2.1: Create Phone Number Helper
10:15-10:45  Step 2.2: Update Authentication DTOs
11:00-12:30  Step 2.3: Update Authentication Service
13:30-14:30  Step 2.4: Update Auth Controller
```

#### Day 3 (6 hours): External Data Service Part 1
```
09:00-10:00  Step 3.1: Create External Data DTOs
10:15-11:15  Step 3.2: Create Integration Service Interface
11:30-17:30  Step 3.3: Implement Integration Service (start)
```

#### Day 4 (4 hours): External Data Service Part 2
```
09:00-12:00  Step 3.3: Implement Integration Service (finish)
13:00-13:30  Step 3.4: Add Service Registration
13:45-15:15  Step 3.5: Create Unit Tests
```

#### Day 5 (6 hours): API Controllers
```
09:00-12:00  Step 4.1: Create External Data Controller
13:00-15:00  Step 4.2: Create Data Management Controller
15:15-16:15  Step 4.3: Update API Documentation
```

#### Day 6 (4 hours): Testing & QA
```
09:00-11:00  Step 5.1: Integration Testing
11:15-12:15  Step 5.2: Performance Testing
13:00-14:00  Step 5.3: Security & Final Validation
```

### Prerequisites & Dependencies

#### Before Starting:
- [ ] Backup current database
- [ ] Ensure development environment is ready
- [ ] Have sample CSV data for testing
- [ ] Prepare Postman collection for API testing

#### Tools Required:
- [ ] Entity Framework Core CLI tools
- [ ] SQL Server Management Studio (for database inspection)
- [ ] Postman or similar API testing tool
- [ ] Unit testing framework (xUnit)

### Risk Mitigation

#### Potential Issues:
1. **Database Migration Conflicts**
   - Solution: Create migrations incrementally, test each step
   
2. **Performance Issues with Bulk Import**
   - Solution: Implement batch processing, add monitoring
   
3. **Phone Number Format Variations**
   - Solution: Comprehensive testing with various formats
   
4. **Authentication Breaking Changes**
   - Solution: Maintain backward compatibility, gradual rollout

### Success Criteria

#### Completion Checklist:
- [ ] All entity changes applied without data loss
- [ ] Mobile phone authentication working end-to-end
- [ ] Bulk import can process 1000+ records in <30 seconds
- [ ] All API endpoints documented and tested
- [ ] Integration tests passing at 95%+ success rate
- [ ] No security vulnerabilities identified
- [ ] Performance metrics meet requirements

---

## Testing Requirements

### Unit Tests
- Phone number normalization และ validation
- External data transformation logic
- Authentication service updates
- Data validation rules

### Integration Tests
- Mobile phone authentication flow
- Bulk data import scenarios
- API endpoint functionality
- Database operations

### Performance Tests
- Bulk import performance (1000+ records)
- Concurrent API requests
- Database query optimization

---

## Security Considerations

### Mobile Phone Authentication
- Rate limiting สำหรับ OTP requests
- Phone number verification
- Secure token generation

### External Data Integration
- API key authentication สำหรับ webhook
- Input validation และ sanitization
- Audit logging สำหรับ data changes
- Access control สำหรับ sensitive operations

---

## Deployment Notes

### Database Changes
- Run migrations ใน production environment
- Backup existing data ก่อน migration
- Monitor performance หลัง deployment

### Configuration Updates
- เพิ่ม external system configurations
- อัปเดต JWT settings สำหรับ mobile authentication
- Configure logging levels

### Monitoring
- ติดตาม bulk import performance
- Monitor authentication errors
- Track external data sync status

---

## Total Estimated Hours: 26 hours
- Task 2.4: 6 hours
- Task 4.7: 4 hours  
- Task 4.10: 10 hours
- Task 5.8: 6 hours

**Project Total: 153 hours (existing) + 26 hours = 179 hours**