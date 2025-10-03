using System.Text.RegularExpressions;
using Application.DTOs.ExternalData;
using Application.Helpers;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Service สำหรับจัดการการรับและประมวลผลข้อมูล Alumni จากระบบภายนอก
/// </summary>
public class ExternalDataIntegrationService : IExternalDataIntegrationService
{
    private readonly IUserRepository _userRepository;
    private readonly IAlumniProfileRepository _profileRepository;
    private readonly ILogger<ExternalDataIntegrationService> _logger;

    // Regex patterns for validation
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$", 
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public ExternalDataIntegrationService(
        IUserRepository userRepository,
        IAlumniProfileRepository profileRepository,
        ILogger<ExternalDataIntegrationService> logger)
    {
        _userRepository = userRepository;
        _profileRepository = profileRepository;
        _logger = logger;
    }

    #region Bulk Data Processing

    public async Task<ImportResult> ProcessBulkDataAsync(BulkImportRequest request, CancellationToken cancellationToken = default)
    {
        var result = new ImportResult
        {
            TotalRecords = request.Alumni.Count,
            StartedAt = DateTime.UtcNow,
            ProcessedAt = DateTime.UtcNow,
            ExternalSystemId = request.ExternalSystemId,
            BatchId = Guid.NewGuid().ToString()
        };

        _logger.LogInformation("Starting bulk import for {Count} records from system {SystemId}", 
            request.Alumni.Count, request.ExternalSystemId);

        try
        {
            // Detect duplicates within the batch
            var cleanedData = DetectAndHandleDuplicates(request.Alumni, result);
            
            if (request.ValidateOnly)
            {
                var validationResult = await ValidateDataAsync(cleanedData, request.ExternalSystemId, cancellationToken);
                result.Errors.AddRange(validationResult.Errors);
                result.Warnings.AddRange(validationResult.Warnings);
            }
            else
            {
                // Process in batches to avoid memory issues
                await ProcessBatchDataInternalAsync(cleanedData, request, result, cancellationToken);
            }

            result.ProcessedAt = DateTime.UtcNow;
            result.ProcessingDuration = result.ProcessedAt - result.StartedAt;

            _logger.LogInformation("Bulk import completed: {Success} success, {Failed} failed, {Skipped} skipped", 
                result.SuccessfulImports, result.FailedImports, result.SkippedRecords);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during bulk import for system {SystemId}", request.ExternalSystemId);
            result.Errors.Add(ImportError.Create("SYSTEM", "ProcessBulkData", ex.Message, errorCode: "SYSTEM_ERROR"));
            result.FailedImports = result.TotalRecords - result.SuccessfulImports;
        }

        return result;
    }

    public Task<ValidationResult> ValidateDataAsync(List<ExternalAlumniData> data, string externalSystemId, CancellationToken cancellationToken = default)
    {
        var result = new ImportResult { ExternalSystemId = externalSystemId };
        var validRecords = 0;

        _logger.LogInformation("Validating {Count} records for system {SystemId}", data.Count, externalSystemId);

        foreach (var alumniData in data)
        {
            try
            {
                if (ValidateAlumniData(alumniData, result))
                {
                    validRecords++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating alumni record {MemberID}", alumniData.MemberID);
                result.Errors.Add(ImportError.Create(
                    alumniData.MemberID ?? "UNKNOWN", 
                    "General", 
                    ex.Message, 
                    errorCode: "VALIDATION_EXCEPTION"));
            }
        }

        return Task.FromResult(new ValidationResult
        {
            IsValid = result.Errors.Count == 0,
            Errors = result.Errors,
            Warnings = result.Warnings,
            ValidRecords = validRecords,
            InvalidRecords = data.Count - validRecords
        });
    }

    public async Task<ImportResult> ProcessBatchDataAsync(
        IEnumerable<ExternalAlumniData> data, 
        string externalSystemId, 
        int batchSize = 100, 
        bool overwriteExisting = false,
        CancellationToken cancellationToken = default)
    {
        var request = new BulkImportRequest
        {
            Alumni = data.ToList(),
            ExternalSystemId = externalSystemId,
            OverwriteExisting = overwriteExisting,
            BatchSize = batchSize
        };

        return await ProcessBulkDataAsync(request, cancellationToken);
    }

    #endregion

    #region Single Record Processing

    public async Task<bool> SyncSingleRecordAsync(ExternalAlumniData data, string externalSystemId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = new ImportResult { ExternalSystemId = externalSystemId };
            
            // Validate data first
            if (!ValidateAlumniData(data, result))
            {
                _logger.LogWarning("Invalid data for member {MemberID}: {Errors}", 
                    data.MemberID ?? "UNKNOWN", string.Join(", ", result.Errors.Select(e => e.Error)));
                return false;
            }

            await ProcessSingleRecordAsync(data, externalSystemId, true, result, cancellationToken);
            
            return result.FailedImports == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing single record for member {MemberID}", data.MemberID);
            return false;
        }
    }

    public async Task<bool> UpdateSingleRecordAsync(string memberID, ExternalAlumniData data, string externalSystemId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Ensure member ID matches
            data.MemberID = memberID;
            
            return await SyncSingleRecordAsync(data, externalSystemId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating single record for member {MemberID}", memberID);
            return false;
        }
    }

    #endregion

    #region Data Validation

    public bool ValidateAlumniData(ExternalAlumniData data, ImportResult result)
    {
        var isValid = true;

        // Required field validation
        if (string.IsNullOrWhiteSpace(data.MemberID))
        {
            result.Errors.Add(ImportError.Create(
                data.MemberID ?? "UNKNOWN", 
                "MemberID", 
                "MemberID is required", 
                errorCode: "REQUIRED_FIELD"));
            isValid = false;
        }

        // Must have either name or firstname
        if (string.IsNullOrWhiteSpace(data.NameInYearbook) && 
            string.IsNullOrWhiteSpace(data.Firstname))
        {
            result.Errors.Add(ImportError.Create(
                data.MemberID ?? "UNKNOWN", 
                "Name", 
                "Either NameInYearbook or Firstname is required", 
                errorCode: "REQUIRED_FIELD"));
            isValid = false;
        }

        // Mobile phone validation
        if (!string.IsNullOrWhiteSpace(data.MobilePhone))
        {
            var normalizedPhone = ValidateAndNormalizePhoneNumber(data.MobilePhone, data.MemberID ?? "UNKNOWN", result);
            if (normalizedPhone == null)
            {
                isValid = false;
            }
        }

        // Email validation
        if (!string.IsNullOrWhiteSpace(data.Email) && !IsValidEmail(data.Email))
        {
            result.Errors.Add(ImportError.Create(
                data.MemberID ?? "UNKNOWN", 
                "Email", 
                "Invalid email format", 
                data.Email, 
                "INVALID_FORMAT"));
            isValid = false;
        }

        // Graduation year validation
        if (data.GraduationYear.HasValue && 
            (data.GraduationYear < 1950 || data.GraduationYear > DateTime.Now.Year + 10))
        {
            result.Warnings.Add(ImportWarning.Create(
                data.MemberID ?? "UNKNOWN", 
                "GraduationYear", 
                "Graduation year seems unusual", 
                data.GraduationYear.ToString()));
        }

        // Additional business rule validations can be added here
        ValidateBusinessRules(data, result);

        return isValid;
    }

    public string? ValidateAndNormalizePhoneNumber(string? phoneNumber, string memberID, ImportResult result)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return null;
        }

        try
        {
            return PhoneNumberHelper.NormalizeMobilePhone(phoneNumber);
        }
        catch (Exception ex)
        {
            result.Errors.Add(ImportError.Create(
                memberID, 
                "MobilePhone", 
                ex.Message, 
                phoneNumber, 
                "INVALID_PHONE_FORMAT"));
            return null;
        }
    }

    public bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return true; // Email is optional
        }

        return EmailRegex.IsMatch(email);
    }

    #endregion

    #region Duplicate Detection

    public async Task<User?> FindExistingUserAsync(string memberID, string? normalizedMobile, CancellationToken cancellationToken = default)
    {
        // Try to find by member ID first
        var user = await _userRepository.GetByMemberIDAsync(memberID, cancellationToken);
        
        if (user == null && !string.IsNullOrWhiteSpace(normalizedMobile))
        {
            // Try to find by mobile phone
            user = await _userRepository.GetByMobilePhoneAsync(normalizedMobile, cancellationToken);
        }

        return user;
    }

    public List<ExternalAlumniData> DetectAndHandleDuplicates(List<ExternalAlumniData> data, ImportResult result)
    {
        var uniqueData = new List<ExternalAlumniData>();
        var seenMemberIDs = new HashSet<string>();
        var seenPhones = new HashSet<string>();

        foreach (var item in data)
        {
            var isDuplicate = false;

            // Check for duplicate MemberID
            if (seenMemberIDs.Contains(item.MemberID))
            {
                result.Warnings.Add(ImportWarning.Create(
                    item.MemberID, 
                    "MemberID", 
                    "Duplicate MemberID found in batch, skipping duplicate"));
                isDuplicate = true;
            }

            // Check for duplicate mobile phone
            if (!string.IsNullOrWhiteSpace(item.MobilePhone))
            {
                try
                {
                    var normalizedPhone = PhoneNumberHelper.NormalizeMobilePhone(item.MobilePhone);
                    if (seenPhones.Contains(normalizedPhone))
                    {
                        result.Warnings.Add(ImportWarning.Create(
                            item.MemberID, 
                            "MobilePhone", 
                            "Duplicate mobile phone found in batch", 
                            item.MobilePhone));
                        isDuplicate = true;
                    }
                    else
                    {
                        seenPhones.Add(normalizedPhone);
                    }
                }
                catch
                {
                    // Phone validation will catch this later
                }
            }

            if (!isDuplicate)
            {
                uniqueData.Add(item);
                seenMemberIDs.Add(item.MemberID);
            }
            else
            {
                result.SkippedRecords++;
            }
        }

        return uniqueData;
    }

    #endregion

    #region User and Profile Management

    public async Task<User> CreateNewUserWithProfileAsync(
        ExternalAlumniData data, 
        string externalSystemId, 
        string? normalizedMobile,
        CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            MobilePhone = normalizedMobile ?? string.Empty,
            Email = data.Email ?? string.Empty,
            Firstname = GetFirstname(data),
            Lastname = GetLastname(data),
            MemberID = data.MemberID ?? "UNKNOWN",
            NameInYearbook = GetDisplayName(data),
            RoleId = 1, // Member role
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        // Create alumni profile
        var profile = new AlumniProfile
        {
            UserId = createdUser.Id,
            ExternalMemberID = data.MemberID ?? "UNKNOWN",
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
            District = data.District,
            Province = data.Province,
            Country = data.Country ?? "Thailand",
            CompanyName = data.CompanyName,
            JobTitle = data.JobTitle,
            WorkAddress = data.WorkAddress,
            MaritalStatus = data.MaritalStatus,
            Status = data.Status,
            SpouseName = data.SpouseName,
            Comment = data.Comment,
            DateOfBirth = data.DateOfBirth,
            GraduationYear = data.GraduationYear,
            Major = data.Major,
            ClassName = data.ClassName,
            CreatedAt = DateTime.UtcNow
        };

        await _profileRepository.AddAsync(profile, cancellationToken);
        await _profileRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created new user and profile for member {MemberID}", data.MemberID);
        return createdUser;
    }

    public async Task<User> UpdateExistingUserWithProfileAsync(
        User existingUser, 
        ExternalAlumniData data, 
        string externalSystemId,
        CancellationToken cancellationToken = default)
    {
        // Update user data
        existingUser.Firstname = GetFirstname(data);
        existingUser.Lastname = GetLastname(data);
        existingUser.NameInYearbook = GetDisplayName(data);
        if (!string.IsNullOrWhiteSpace(data.Email))
        {
            existingUser.Email = data.Email;
        }
        
        existingUser.MemberID = data.MemberID;
        existingUser.UpdateTimestamp();

        await _userRepository.UpdateAsync(existingUser, cancellationToken);

        // Update or create alumni profile
        var profile = await _profileRepository.GetByUserIdAsync(existingUser.Id, cancellationToken);
        
        if (profile == null)
        {
            // Create new profile
            profile = new AlumniProfile
            {
                UserId = existingUser.Id,
                CreatedAt = DateTime.UtcNow
            };
        }

        // Update profile data
        UpdateProfileFromExternalData(profile, data, externalSystemId);
        
        if (profile.Id == 0)
        {
            await _profileRepository.AddAsync(profile, cancellationToken);
        }
        else
        {
            await _profileRepository.UpdateAsync(profile, cancellationToken);
        }
        
        await _profileRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated existing user and profile for member {MemberID}", data.MemberID);
        return existingUser;
    }

    #endregion

    #region Statistics and Monitoring

    public async Task<ImportStatistics> GetImportStatisticsAsync(
        string? externalSystemId = null, 
        DateTime? fromDate = null, 
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement statistics collection from audit logs or dedicated statistics table
        // For now, return mock data
        await Task.Delay(100, cancellationToken);

        return new ImportStatistics
        {
            TotalImports = 0,
            SuccessfulImports = 0,
            FailedImports = 0,
            SuccessRate = 0,
            LastImportDate = null,
            AverageProcessingTime = TimeSpan.Zero
        };
    }

    public async Task<List<User>> GetUsersNeedingSyncAsync(
        string? externalSystemId = null, 
        int olderThanHours = 24,
        CancellationToken cancellationToken = default)
    {
        // In the new schema, we don't track sync status, so return all alumni members
        return await _userRepository.GetAlumniMembersAsync(cancellationToken);
    }

    #endregion

    #region Private Helper Methods

    private async Task ProcessBatchDataInternalAsync(
        List<ExternalAlumniData> data, 
        BulkImportRequest request, 
        ImportResult result,
        CancellationToken cancellationToken)
    {
        var batches = data.Chunk(request.BatchSize);

        foreach (var batch in batches)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Bulk import cancelled for system {SystemId}", request.ExternalSystemId);
                break;
            }

            await ProcessBatchInternalAsync(batch.ToList(), request, result, cancellationToken);
        }
    }

    private async Task ProcessBatchInternalAsync(
        List<ExternalAlumniData> batch, 
        BulkImportRequest request, 
        ImportResult result,
        CancellationToken cancellationToken)
    {
        foreach (var alumniData in batch)
        {
            try
            {
                await ProcessSingleRecordAsync(alumniData, request.ExternalSystemId, request.OverwriteExisting, result, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing alumni record {MemberID}", alumniData.MemberID);
                result.Errors.Add(ImportError.Create(
                    alumniData.MemberID ?? "UNKNOWN", 
                    "General", 
                    ex.Message, 
                    errorCode: "PROCESSING_ERROR"));
                result.FailedImports++;
            }
        }
    }

    private async Task ProcessSingleRecordAsync(
        ExternalAlumniData data, 
        string externalSystemId, 
        bool overwriteExisting, 
        ImportResult result,
        CancellationToken cancellationToken)
    {
        // Validate data first
        if (!ValidateAlumniData(data, result))
        {
            result.FailedImports++;
            return;
        }

        // Normalize phone number
        string? normalizedMobile = null;
        if (!string.IsNullOrWhiteSpace(data.MobilePhone))
        {
            normalizedMobile = ValidateAndNormalizePhoneNumber(data.MobilePhone, data.MemberID ?? "UNKNOWN", result);
            if (normalizedMobile == null)
            {
                result.FailedImports++;
                return;
            }
        }

        // Find existing user
        var existingUser = await FindExistingUserAsync(data.MemberID ?? "UNKNOWN", normalizedMobile, cancellationToken);

        if (existingUser != null && !overwriteExisting)
        {
            result.SkippedRecords++;
            return;
        }

        if (existingUser == null)
        {
            // Create new user and profile
            await CreateNewUserWithProfileAsync(data, externalSystemId, normalizedMobile, cancellationToken);
            result.SuccessfulImports++;
            result.NewRecords++;
        }
        else
        {
            // Update existing user and profile
            await UpdateExistingUserWithProfileAsync(existingUser, data, externalSystemId, cancellationToken);
            result.SuccessfulImports++;
            result.UpdatedRecords++;
        }
    }

    private static string GetDisplayName(ExternalAlumniData data)
    {
        if (!string.IsNullOrWhiteSpace(data.NameInYearbook))
        {
            return data.NameInYearbook;
        }

        return $"{data.Firstname} {data.Lastname}".Trim();
    }

    private static string GetFirstname(ExternalAlumniData data)
    {
        return data.Firstname ?? string.Empty;
    }

    private static string GetLastname(ExternalAlumniData data)
    {
        return data.Lastname ?? string.Empty;
    }

    private static void UpdateProfileFromExternalData(AlumniProfile profile, ExternalAlumniData data, string externalSystemId)
    {
        profile.ExternalMemberID = data.MemberID;
        profile.ExternalSystemId = externalSystemId;
        profile.ExternalDataLastSync = DateTime.UtcNow;
        
        // Update all profile fields
        profile.NameInYearbook = data.NameInYearbook;
        profile.TitleCode = data.TitleID;
        profile.Firstname = data.Firstname;
        profile.Lastname = data.Lastname;
        profile.NickName = data.NickName;
        profile.GroupCode = data.GroupID;
        profile.Phone = data.Phone;
        profile.MobilePhone = data.MobilePhone;
        profile.LineID = data.LineID;
        profile.Facebook = data.Facebook;
        profile.Email = data.Email;
        profile.Address = data.Address;
        profile.ZipCode = data.ZipCode;
        profile.District = data.District;
        profile.Province = data.Province;
        profile.Country = data.Country ?? "Thailand";
        profile.CompanyName = data.CompanyName;
        profile.JobTitle = data.JobTitle;
        profile.WorkAddress = data.WorkAddress;
        profile.MaritalStatus = data.MaritalStatus;
        profile.Status = data.Status;
        profile.SpouseName = data.SpouseName;
        profile.Comment = data.Comment;
        profile.DateOfBirth = data.DateOfBirth;
        profile.GraduationYear = data.GraduationYear;
        profile.Major = data.Major;
        profile.ClassName = data.ClassName;
        
        profile.UpdateTimestamp();
    }

    private static void ValidateBusinessRules(ExternalAlumniData data, ImportResult result)
    {
        // Example business rule: Check if graduation year matches with age
        if (data.DateOfBirth.HasValue && data.GraduationYear.HasValue)
        {
            var estimatedGraduationAge = data.GraduationYear.Value - data.DateOfBirth.Value.Year;
            if (estimatedGraduationAge < 18 || estimatedGraduationAge > 35)
            {
                result.Warnings.Add(ImportWarning.Create(
                    data.MemberID ?? "UNKNOWN", 
                    "AgeValidation", 
                    "Age at graduation seems unusual", 
                    $"{estimatedGraduationAge} years old"));
            }
        }

        // Example: Validate company name format
        if (!string.IsNullOrWhiteSpace(data.CompanyName) && data.CompanyName.Length > 200)
        {
            result.Warnings.Add(ImportWarning.Create(
                data.MemberID ?? "UNKNOWN", 
                "CompanyName", 
                "Company name is very long", 
                data.CompanyName,
                data.CompanyName.Substring(0, 200)));
        }
    }

    #endregion
}
