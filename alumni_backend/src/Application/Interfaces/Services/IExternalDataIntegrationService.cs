using Application.DTOs.ExternalData;

namespace Application.Interfaces.Services;

/// <summary>
/// Service สำหรับจัดการการรับข้อมูล Alumni จากระบบภายนอก
/// </summary>
public interface IExternalDataIntegrationService
{
    #region Bulk Data Processing

    /// <summary>
    /// ประมวลผลข้อมูลแบบ bulk จากระบบภายนอก
    /// </summary>
    /// <param name="request">ข้อมูลการร้องขอ import</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ผลลัพธ์การ import</returns>
    Task<ImportResult> ProcessBulkDataAsync(BulkImportRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// ตรวจสอบความถูกต้องของข้อมูลโดยไม่ import จริง
    /// </summary>
    /// <param name="data">ข้อมูล alumni ที่ต้องการตรวจสอบ</param>
    /// <param name="externalSystemId">รหัสระบบภายนอก</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ผลการตรวจสอบ</returns>
    Task<ValidationResult> ValidateDataAsync(List<ExternalAlumniData> data, string externalSystemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ประมวลผลข้อมูลแบบ batch เพื่อหลีกเลี่ยง memory overflow
    /// </summary>
    /// <param name="data">ข้อมูล alumni</param>
    /// <param name="externalSystemId">รหัสระบบภายนอก</param>
    /// <param name="batchSize">ขนาด batch</param>
    /// <param name="overwriteExisting">เขียนทับข้อมูลเดิมหรือไม่</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ผลลัพธ์การ import</returns>
    Task<ImportResult> ProcessBatchDataAsync(
        IEnumerable<ExternalAlumniData> data, 
        string externalSystemId, 
        int batchSize = 100, 
        bool overwriteExisting = false,
        CancellationToken cancellationToken = default);

    #endregion

    #region Single Record Processing

    /// <summary>
    /// Sync ข้อมูล alumni รายคนจากระบบภายนอก
    /// </summary>
    /// <param name="data">ข้อมูล alumni</param>
    /// <param name="externalSystemId">รหัสระบบภายนอก</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True ถ้า sync สำเร็จ</returns>
    Task<bool> SyncSingleRecordAsync(ExternalAlumniData data, string externalSystemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// อัปเดตข้อมูล alumni รายคน
    /// </summary>
    /// <param name="memberID">รหัสสมาชิก</param>
    /// <param name="data">ข้อมูลใหม่</param>
    /// <param name="externalSystemId">รหัสระบบภายนอก</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True ถ้าอัปเดตสำเร็จ</returns>
    Task<bool> UpdateSingleRecordAsync(string memberID, ExternalAlumniData data, string externalSystemId, CancellationToken cancellationToken = default);

    #endregion

    #region Data Validation

    /// <summary>
    /// ตรวจสอบความถูกต้องของข้อมูล alumni รายคน
    /// </summary>
    /// <param name="data">ข้อมูล alumni</param>
    /// <param name="result">ผลลัพธ์การ import (สำหรับเก็บ error/warning)</param>
    /// <returns>True ถ้าข้อมูลถูกต้อง</returns>
    bool ValidateAlumniData(ExternalAlumniData data, ImportResult result);

    /// <summary>
    /// ตรวจสอบรูปแบบหมายเลขโทรศัพท์
    /// </summary>
    /// <param name="phoneNumber">หมายเลขโทรศัพท์</param>
    /// <param name="memberID">รหัสสมาชิก (สำหรับ error reporting)</param>
    /// <param name="result">ผลลัพธ์การ import</param>
    /// <returns>หมายเลขโทรศัพท์ที่ normalize แล้ว หรือ null ถ้าไม่ถูกต้อง</returns>
    string? ValidateAndNormalizePhoneNumber(string? phoneNumber, string memberID, ImportResult result);

    /// <summary>
    /// ตรวจสอบ email format
    /// </summary>
    /// <param name="email">Email address</param>
    /// <returns>True ถ้า email ถูกต้อง</returns>
    bool IsValidEmail(string? email);

    #endregion

    #region Duplicate Detection

    /// <summary>
    /// ค้นหา User ที่มีอยู่แล้วจาก external member ID หรือ mobile phone
    /// </summary>
    /// <param name="externalMemberID">รหัสสมาชิกจากระบบภายนอก</param>
    /// <param name="normalizedMobile">หมายเลขโทรศัพท์ที่ normalize แล้ว</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User ที่พบ หรือ null</returns>
    Task<Domain.Entities.User?> FindExistingUserAsync(string externalMemberID, string? normalizedMobile, CancellationToken cancellationToken = default);

    /// <summary>
    /// ตรวจสอบข้อมูลซ้ำซ้อนใน batch
    /// </summary>
    /// <param name="data">ข้อมูล alumni ทั้งหมด</param>
    /// <param name="result">ผลลัพธ์การ import</param>
    /// <returns>ข้อมูลที่ไม่ซ้ำซ้อน</returns>
    List<ExternalAlumniData> DetectAndHandleDuplicates(List<ExternalAlumniData> data, ImportResult result);

    #endregion

    #region User and Profile Management

    /// <summary>
    /// สร้าง User และ AlumniProfile ใหม่จากข้อมูลภายนอก
    /// </summary>
    /// <param name="data">ข้อมูล alumni</param>
    /// <param name="externalSystemId">รหัสระบบภายนอก</param>
    /// <param name="normalizedMobile">หมายเลขโทรศัพท์ที่ normalize แล้ว</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User ที่สร้างขึ้น</returns>
    Task<Domain.Entities.User> CreateNewUserWithProfileAsync(
        ExternalAlumniData data, 
        string externalSystemId, 
        string? normalizedMobile,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// อัปเดต User และ AlumniProfile ที่มีอยู่แล้ว
    /// </summary>
    /// <param name="existingUser">User ที่มีอยู่แล้ว</param>
    /// <param name="data">ข้อมูลใหม่</param>
    /// <param name="externalSystemId">รหัสระบบภายนอก</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User ที่อัปเดตแล้ว</returns>
    Task<Domain.Entities.User> UpdateExistingUserWithProfileAsync(
        Domain.Entities.User existingUser, 
        ExternalAlumniData data, 
        string externalSystemId,
        CancellationToken cancellationToken = default);

    #endregion

    #region Statistics and Monitoring

    /// <summary>
    /// ดึงสถิติการ import จากระบบภายนอก
    /// </summary>
    /// <param name="externalSystemId">รหัสระบบภายนอก (optional)</param>
    /// <param name="fromDate">วันที่เริ่มต้น (optional)</param>
    /// <param name="toDate">วันที่สิ้นสุด (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>สถิติการ import</returns>
    Task<ImportStatistics> GetImportStatisticsAsync(
        string? externalSystemId = null, 
        DateTime? fromDate = null, 
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ดึงรายการ User ที่ต้องการ sync อีกครั้ง
    /// </summary>
    /// <param name="externalSystemId">รหัสระบบภายนอก (optional)</param>
    /// <param name="olderThanHours">sync ล่าสุดเกิน n ชั่วโมง</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>รายการ User ที่ต้อง sync</returns>
    Task<List<Domain.Entities.User>> GetUsersNeedingSyncAsync(
        string? externalSystemId = null, 
        int olderThanHours = 24,
        CancellationToken cancellationToken = default);

    #endregion
}

/// <summary>
/// สถิติการ import ข้อมูล
/// </summary>
public class ImportStatistics
{
    public int TotalImports { get; set; }
    public int SuccessfulImports { get; set; }
    public int FailedImports { get; set; }
    public double SuccessRate { get; set; }
    public DateTime? LastImportDate { get; set; }
    public TimeSpan AverageProcessingTime { get; set; }
    public Dictionary<string, int> SystemBreakdown { get; set; } = new();
    public Dictionary<string, int> ErrorBreakdown { get; set; } = new();
}