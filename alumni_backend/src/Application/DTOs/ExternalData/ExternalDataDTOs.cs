using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.ExternalData;

/// <summary>
/// DTO สำหรับข้อมูล Alumni จากระบบภายนอก (CSV data structure)
/// </summary>
public class ExternalAlumniData
{
    [Required(ErrorMessage = "Member ID is required")]
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
    
    /// <summary>
    /// Additional fields for enhanced data
    /// </summary>
    public string? District { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; } = "Thailand";
    public string? JobTitle { get; set; }
    public string? WorkAddress { get; set; }
    public string? MaritalStatus { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? GraduationYear { get; set; }
    public string? Major { get; set; }
    public string? ClassName { get; set; }
    
    /// <summary>
    /// Gets full name from firstname and lastname
    /// </summary>
    public string GetFullName()
    {
        return $"{Firstname} {Lastname}".Trim();
    }
    
    /// <summary>
    /// Validates if the data has minimum required fields
    /// </summary>
    public bool HasMinimumRequiredData()
    {
        return !string.IsNullOrWhiteSpace(MemberID) &&
               (!string.IsNullOrWhiteSpace(Firstname) || !string.IsNullOrWhiteSpace(NameInYearbook));
    }
}

/// <summary>
/// Request สำหรับการ import ข้อมูลแบบ bulk
/// </summary>
public class BulkImportRequest
{
    [Required(ErrorMessage = "External System ID is required")]
    public string ExternalSystemId { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Alumni data is required")]
    [MinLength(1, ErrorMessage = "At least one alumni record is required")]
    public List<ExternalAlumniData> Alumni { get; set; } = new();
    
    /// <summary>
    /// Whether to overwrite existing records with same MemberID
    /// </summary>
    public bool OverwriteExisting { get; set; } = false;
    
    /// <summary>
    /// Only validate data without actually importing
    /// </summary>
    public bool ValidateOnly { get; set; } = false;
    
    /// <summary>
    /// Batch size for processing (default 100)
    /// </summary>
    [Range(1, 1000, ErrorMessage = "Batch size must be between 1 and 1000")]
    public int BatchSize { get; set; } = 100;
    
    /// <summary>
    /// Skip invalid records and continue processing
    /// </summary>
    public bool SkipInvalidRecords { get; set; } = true;
    
    /// <summary>
    /// Send notification upon completion
    /// </summary>
    public bool NotifyOnCompletion { get; set; } = false;
    
    /// <summary>
    /// Email for notifications (if NotifyOnCompletion is true)
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? NotificationEmail { get; set; }
}

/// <summary>
/// ผลลัพธ์การ import ข้อมูล
/// </summary>
public class ImportResult
{
    public int TotalRecords { get; set; }
    public int SuccessfulImports { get; set; }
    public int FailedImports { get; set; }
    public int SkippedRecords { get; set; }
    public int UpdatedRecords { get; set; }
    public int NewRecords { get; set; }
    public List<ImportError> Errors { get; set; } = new();
    public List<ImportWarning> Warnings { get; set; } = new();
    public DateTime ProcessedAt { get; set; }
    public DateTime StartedAt { get; set; }
    public TimeSpan ProcessingDuration { get; set; }
    public string ExternalSystemId { get; set; } = string.Empty;
    public string? BatchId { get; set; }
    
    /// <summary>
    /// Summary statistics
    /// </summary>
    public ImportSummary Summary => new()
    {
        SuccessRate = TotalRecords > 0 ? (double)SuccessfulImports / TotalRecords * 100 : 0,
        ErrorRate = TotalRecords > 0 ? (double)FailedImports / TotalRecords * 100 : 0,
        HasErrors = Errors.Any(),
        HasWarnings = Warnings.Any(),
        IsCompleteSuccess = FailedImports == 0 && Errors.Count == 0
    };
}

/// <summary>
/// Summary ของผลการ import
/// </summary>
public class ImportSummary
{
    public double SuccessRate { get; set; }
    public double ErrorRate { get; set; }
    public bool HasErrors { get; set; }
    public bool HasWarnings { get; set; }
    public bool IsCompleteSuccess { get; set; }
}

/// <summary>
/// Error ที่เกิดขึ้นระหว่างการ import
/// </summary>
public class ImportError
{
    public string MemberID { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public string? ReceivedValue { get; set; }
    public string ErrorCode { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Severity { get; set; } = "Error"; // Error, Warning, Critical
    
    public static ImportError Create(string memberID, string field, string error, string? receivedValue = null, string errorCode = "VALIDATION_ERROR")
    {
        return new ImportError
        {
            MemberID = memberID,
            Field = field,
            Error = error,
            ReceivedValue = receivedValue,
            ErrorCode = errorCode,
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Warning ที่เกิดขึ้นระหว่างการ import
/// </summary>
public class ImportWarning
{
    public string MemberID { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string Warning { get; set; } = string.Empty;
    public string? ReceivedValue { get; set; }
    public string? SuggestedValue { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public static ImportWarning Create(string memberID, string field, string warning, string? receivedValue = null, string? suggestedValue = null)
    {
        return new ImportWarning
        {
            MemberID = memberID,
            Field = field,
            Warning = warning,
            ReceivedValue = receivedValue,
            SuggestedValue = suggestedValue,
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Request สำหรับ sync record เดียว
/// </summary>
public class SyncSingleRequest
{
    [Required(ErrorMessage = "External System ID is required")]
    public string ExternalSystemId { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Alumni data is required")]
    public ExternalAlumniData AlumniData { get; set; } = new();
    
    /// <summary>
    /// Force update even if data hasn't changed
    /// </summary>
    public bool ForceUpdate { get; set; } = false;
}

/// <summary>
/// Webhook payload สำหรับรับข้อมูลจากระบบภายนอก
/// </summary>
public class WebhookPayload
{
    [Required(ErrorMessage = "Records are required")]
    public List<ExternalAlumniData> Records { get; set; } = new();
    
    public bool OverwriteExisting { get; set; } = false;
    
    [Required(ErrorMessage = "Event type is required")]
    public string EventType { get; set; } = "bulk_update"; // bulk_update, single_update, delete
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Webhook source identifier
    /// </summary>
    public string? Source { get; set; }
    
    /// <summary>
    /// Webhook signature for verification
    /// </summary>
    public string? Signature { get; set; }
    
    /// <summary>
    /// Additional metadata
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Data validation result
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<ImportError> Errors { get; set; } = new();
    public List<ImportWarning> Warnings { get; set; } = new();
    public int ValidRecords { get; set; }
    public int InvalidRecords { get; set; }
    public DateTime ValidatedAt { get; set; } = DateTime.UtcNow;
    
    public static ValidationResult Success(int validRecords, List<ImportWarning>? warnings = null)
    {
        return new ValidationResult
        {
            IsValid = true,
            ValidRecords = validRecords,
            Warnings = warnings ?? new List<ImportWarning>()
        };
    }
    
    public static ValidationResult Failure(List<ImportError> errors, int validRecords = 0, int invalidRecords = 0)
    {
        return new ValidationResult
        {
            IsValid = false,
            Errors = errors,
            ValidRecords = validRecords,
            InvalidRecords = invalidRecords
        };
    }
}