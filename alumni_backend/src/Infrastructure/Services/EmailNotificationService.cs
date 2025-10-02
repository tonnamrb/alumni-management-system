using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Infrastructure.Services;

public class EmailNotificationService : INotificationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailNotificationService> _logger;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly string _username;
    private readonly string _password;
    private readonly bool _enableSsl;

    public EmailNotificationService(
        IConfiguration configuration,
        ILogger<EmailNotificationService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // ใช้ Gmail SMTP เป็น default (สามารถเปลี่ยนได้ใน appsettings)
        _smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        _fromEmail = _configuration["Email:FromEmail"] ?? throw new InvalidOperationException("Email FromEmail not configured");
        _fromName = _configuration["Email:FromName"] ?? "Alumni System";
        _username = _configuration["Email:Username"] ?? _fromEmail;
        _password = _configuration["Email:Password"] ?? throw new InvalidOperationException("Email Password not configured");
        _enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true");
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = CreateSmtpClient();
            using var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };

            message.To.Add(new MailAddress(to));

            await client.SendMailAsync(message, cancellationToken);
            
            _logger.LogInformation("Email sent successfully to {To} with subject: {Subject}", to, subject);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To} with subject: {Subject}", to, subject);
            return false;
        }
    }

    public async Task<bool> SendEmailToMultipleAsync(IEnumerable<string> to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = CreateSmtpClient();
            using var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };

            foreach (var email in to)
            {
                message.To.Add(new MailAddress(email));
            }

            await client.SendMailAsync(message, cancellationToken);
            
            _logger.LogInformation("Email sent successfully to {Count} recipients with subject: {Subject}", to.Count(), subject);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to multiple recipients with subject: {Subject}", subject);
            return false;
        }
    }

    public async Task<bool> SendWelcomeEmailAsync(string userEmail, string userName, CancellationToken cancellationToken = default)
    {
        var subject = "ยินดีต้อนรับสู่ Alumni System";
        var body = CreateWelcomeEmailBody(userName);
        
        return await SendEmailAsync(userEmail, subject, body, isHtml: true, cancellationToken);
    }

    public async Task<bool> SendNewCommentNotificationAsync(
        string postOwnerEmail,
        string postOwnerName,
        string commenterName,
        string commentContent,
        string postTitle,
        CancellationToken cancellationToken = default)
    {
        var subject = $"มีคอมเมนต์ใหม่ในโพสต์ของคุณ - {postTitle}";
        var body = CreateNewCommentEmailBody(postOwnerName, commenterName, commentContent, postTitle);
        
        return await SendEmailAsync(postOwnerEmail, subject, body, isHtml: true, cancellationToken);
    }

    public async Task<bool> SendNewLikeNotificationAsync(
        string postOwnerEmail,
        string postOwnerName,
        string likerName,
        string postTitle,
        CancellationToken cancellationToken = default)
    {
        var subject = $"มีคนกด Like โพสต์ของคุณ - {postTitle}";
        var body = CreateNewLikeEmailBody(postOwnerName, likerName, postTitle);
        
        return await SendEmailAsync(postOwnerEmail, subject, body, isHtml: true, cancellationToken);
    }

    public async Task<bool> SendPasswordResetEmailAsync(string userEmail, string userName, string resetToken, CancellationToken cancellationToken = default)
    {
        var subject = "รีเซ็ตรหัสผ่าน - Alumni System";
        var body = CreatePasswordResetEmailBody(userName, resetToken);
        
        return await SendEmailAsync(userEmail, subject, body, isHtml: true, cancellationToken);
    }

    private SmtpClient CreateSmtpClient()
    {
        var client = new SmtpClient(_smtpHost, _smtpPort)
        {
            Credentials = new NetworkCredential(_username, _password),
            EnableSsl = _enableSsl,
            Timeout = 30000 // 30 seconds
        };
        
        return client;
    }

    private static string CreateWelcomeEmailBody(string userName)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>ยินดีต้อนรับ</title>
</head>
<body style='font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
        <h1 style='color: #333; text-align: center; border-bottom: 2px solid #007bff; padding-bottom: 15px;'>ยินดีต้อนรับสู่ Alumni System</h1>
        
        <p style='font-size: 16px; line-height: 1.6; color: #555;'>เรียน <strong>{userName}</strong></p>
        
        <p style='font-size: 16px; line-height: 1.6; color: #555;'>
            ยินดีต้อนรับเข้าสู่ Alumni System! เราดีใจที่คุณเข้าร่วมเป็นส่วนหนึ่งของชุมชนศิษย์เก่า
        </p>
        
        <p style='font-size: 16px; line-height: 1.6; color: #555;'>
            ตอนนี้คุณสามารถ:
        </p>
        
        <ul style='font-size: 16px; line-height: 1.8; color: #555; padding-left: 20px;'>
            <li>สร้างและแก้ไขโปรไฟล์ของคุณ</li>
            <li>โพสต์เนื้อหาและแชร์ประสบการณ์</li>
            <li>เชื่อมต่อกับเพื่อนศิษย์เก่า</li>
            <li>ติดตามข่าวสารและกิจกรรม</li>
        </ul>
        
        <div style='text-align: center; margin-top: 30px;'>
            <p style='font-size: 14px; color: #888;'>ขอบคุณที่เข้าร่วมกับเรา!</p>
            <p style='font-size: 14px; color: #888;'>Alumni System Team</p>
        </div>
    </div>
</body>
</html>";
    }

    private static string CreateNewCommentEmailBody(string postOwnerName, string commenterName, string commentContent, string postTitle)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>คอมเมนต์ใหม่</title>
</head>
<body style='font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
        <h1 style='color: #333; text-align: center; border-bottom: 2px solid #28a745; padding-bottom: 15px;'>มีคอมเมนต์ใหม่!</h1>
        
        <p style='font-size: 16px; line-height: 1.6; color: #555;'>เรียน <strong>{postOwnerName}</strong></p>
        
        <p style='font-size: 16px; line-height: 1.6; color: #555;'>
            <strong>{commenterName}</strong> ได้แสดงความคิดเห็นในโพสต์ <strong>'{postTitle}'</strong> ของคุณ
        </p>
        
        <div style='background-color: #f8f9fa; padding: 15px; border-left: 4px solid #28a745; margin: 20px 0; border-radius: 4px;'>
            <p style='font-size: 14px; margin: 0; color: #666; font-style: italic;'>'{commentContent}'</p>
        </div>
        
        <div style='text-align: center; margin-top: 30px;'>
            <p style='font-size: 14px; color: #888;'>Alumni System</p>
        </div>
    </div>
</body>
</html>";
    }

    private static string CreateNewLikeEmailBody(string postOwnerName, string likerName, string postTitle)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Like ใหม่</title>
</head>
<body style='font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
        <h1 style='color: #333; text-align: center; border-bottom: 2px solid #dc3545; padding-bottom: 15px;'>❤️ มีคนกด Like!</h1>
        
        <p style='font-size: 16px; line-height: 1.6; color: #555;'>เรียน <strong>{postOwnerName}</strong></p>
        
        <p style='font-size: 16px; line-height: 1.6; color: #555;'>
            <strong>{likerName}</strong> ได้กด Like ในโพสต์ <strong>'{postTitle}'</strong> ของคุณ
        </p>
        
        <div style='text-align: center; margin-top: 30px;'>
            <p style='font-size: 14px; color: #888;'>Alumni System</p>
        </div>
    </div>
</body>
</html>";
    }

    private static string CreatePasswordResetEmailBody(string userName, string resetToken)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>รีเซ็ตรหัสผ่าน</title>
</head>
<body style='font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
        <h1 style='color: #333; text-align: center; border-bottom: 2px solid #ffc107; padding-bottom: 15px;'>รีเซ็ตรหัสผ่าน</h1>
        
        <p style='font-size: 16px; line-height: 1.6; color: #555;'>เรียน <strong>{userName}</strong></p>
        
        <p style='font-size: 16px; line-height: 1.6; color: #555;'>
            คุณได้ขอรีเซ็ตรหัสผ่านสำหรับบัญชี Alumni System ของคุณ
        </p>
        
        <div style='background-color: #fff3cd; padding: 20px; border: 1px solid #ffeaa7; border-radius: 5px; margin: 20px 0; text-align: center;'>
            <p style='margin: 0; font-size: 14px; color: #856404; margin-bottom: 10px;'>รหัสสำหรับรีเซ็ตรหัสผ่าน:</p>
            <p style='margin: 0; font-size: 24px; font-weight: bold; color: #856404; font-family: monospace; letter-spacing: 2px;'>{resetToken}</p>
        </div>
        
        <p style='font-size: 14px; line-height: 1.6; color: #dc3545;'>
            <strong>หมายเหตุ:</strong> รหัสนี้จะหมดอายุใน 30 นาที และใช้ได้เพียงครั้งเดียวเท่านั้น
        </p>
        
        <p style='font-size: 14px; line-height: 1.6; color: #666;'>
            หากคุณไม่ได้ขอรีเซ็ตรหัสผ่าน กรุณาเพิกเฉยต่ออีเมลนี้
        </p>
        
        <div style='text-align: center; margin-top: 30px;'>
            <p style='font-size: 14px; color: #888;'>Alumni System Security Team</p>
        </div>
    </div>
</body>
</html>";
    }
}