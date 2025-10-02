namespace Application.DTOs;

/// <summary>
/// DTO สำหรับผลการอัพโหลดไฟล์
/// </summary>
public class FileUploadResultDto
{
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}

/// <summary>
/// DTO สำหรับไฟล์ของผู้ใช้งาน
/// </summary>
public class UserFileDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string Folder { get; set; } = string.Empty;
}