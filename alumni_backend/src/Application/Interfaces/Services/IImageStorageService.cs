namespace Application.Interfaces.Services;

public interface IImageStorageService
{
    /// <summary>
    /// อัพโหลดรูปภาพไปยัง cloud storage
    /// </summary>
    /// <param name="imageStream">Stream ของรูปภาพ</param>
    /// <param name="fileName">ชื่อไฟล์</param>
    /// <param name="contentType">Content type ของไฟล์ (เช่น image/jpeg)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>URL ของรูปภาพที่อัพโหลดแล้ว</returns>
    Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// อัพโหลดรูปภาพจาก byte array
    /// </summary>
    /// <param name="imageBytes">Byte array ของรูปภาพ</param>
    /// <param name="fileName">ชื่อไฟล์</param>
    /// <param name="contentType">Content type ของไฟล์</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>URL ของรูปภาพที่อัพโหลดแล้ว</returns>
    Task<string> UploadImageAsync(byte[] imageBytes, string fileName, string contentType, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// ลบรูปภาพออกจาก cloud storage
    /// </summary>
    /// <param name="imageUrl">URL ของรูปภาพที่ต้องการลบ</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>สำเร็จหรือไม่</returns>
    Task<bool> DeleteImageAsync(string imageUrl, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// สร้าง pre-signed URL สำหรับอัพโหลดโดยตรงจาก client
    /// </summary>
    /// <param name="fileName">ชื่อไฟล์</param>
    /// <param name="contentType">Content type ของไฟล์</param>
    /// <param name="expirationMinutes">เวลาหมดอายุใน minutes (default 15 นาที)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Pre-signed URL</returns>
    Task<string> GeneratePresignedUploadUrlAsync(string fileName, string contentType, int expirationMinutes = 15, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// ตรวจสอบว่าไฟล์เป็นรูปภาพหรือไม่
    /// </summary>
    /// <param name="contentType">Content type ของไฟล์</param>
    /// <returns>เป็นรูปภาพหรือไม่</returns>
    bool IsValidImageType(string contentType);
    
    /// <summary>
    /// สร้างชื่อไฟล์ที่ unique
    /// </summary>
    /// <param name="originalFileName">ชื่อไฟล์เดิม</param>
    /// <param name="prefix">Prefix ของชื่อไฟล์ (เช่น profile, post)</param>
    /// <returns>ชื่อไฟล์ที่ unique</returns>
    string GenerateUniqueFileName(string originalFileName, string prefix = "");
}