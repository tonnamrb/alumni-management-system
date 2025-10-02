namespace Application.Interfaces.Services;

public interface INotificationService
{
    /// <summary>
    /// ส่งอีเมลแจ้งเตือน
    /// </summary>
    /// <param name="to">อีเมลปลายทาง</param>
    /// <param name="subject">หัวข้อ</param>
    /// <param name="body">เนื้อหา</param>
    /// <param name="isHtml">เป็น HTML หรือไม่</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ส่งสำเร็จหรือไม่</returns>
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// ส่งอีเมลแจ้งเตือนแบบหลายคน
    /// </summary>
    /// <param name="to">รายชื่ออีเมลปลายทาง</param>
    /// <param name="subject">หัวข้อ</param>
    /// <param name="body">เนื้อหา</param>
    /// <param name="isHtml">เป็น HTML หรือไม่</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ส่งสำเร็จหรือไม่</returns>
    Task<bool> SendEmailToMultipleAsync(IEnumerable<string> to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// ส่งอีเมลต้อนรับสมาชิกใหม่
    /// </summary>
    /// <param name="userEmail">อีเมลของ user</param>
    /// <param name="userName">ชื่อของ user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ส่งสำเร็จหรือไม่</returns>
    Task<bool> SendWelcomeEmailAsync(string userEmail, string userName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// ส่งอีเมลแจ้งเตือนเมื่อมี comment ใหม่
    /// </summary>
    /// <param name="postOwnerEmail">อีเมลของเจ้าของโพสต์</param>
    /// <param name="postOwnerName">ชื่อของเจ้าของโพสต์</param>
    /// <param name="commenterName">ชื่อของคนคอมเมนต์</param>
    /// <param name="commentContent">เนื้อหาคอมเมนต์</param>
    /// <param name="postTitle">หัวข้อโพสต์</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ส่งสำเร็จหรือไม่</returns>
    Task<bool> SendNewCommentNotificationAsync(
        string postOwnerEmail, 
        string postOwnerName, 
        string commenterName, 
        string commentContent, 
        string postTitle,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// ส่งอีเมลแจ้งเตือนเมื่อมี like ใหม่
    /// </summary>
    /// <param name="postOwnerEmail">อีเมลของเจ้าของโพสต์</param>
    /// <param name="postOwnerName">ชื่อของเจ้าของโพสต์</param>
    /// <param name="likerName">ชื่อของคนกด like</param>
    /// <param name="postTitle">หัวข้อโพสต์</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ส่งสำเร็จหรือไม่</returns>
    Task<bool> SendNewLikeNotificationAsync(
        string postOwnerEmail, 
        string postOwnerName, 
        string likerName, 
        string postTitle,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// ส่งอีเมลแจ้งเตือนการรีเซ็ตรหัสผ่าน
    /// </summary>
    /// <param name="userEmail">อีเมลของ user</param>
    /// <param name="userName">ชื่อของ user</param>
    /// <param name="resetToken">Token สำหรับรีเซ็ตรหัสผ่าน</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ส่งสำเร็จหรือไม่</returns>
    Task<bool> SendPasswordResetEmailAsync(string userEmail, string userName, string resetToken, CancellationToken cancellationToken = default);
}