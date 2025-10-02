using Application.DTOs;
using System.Net;
using System.Text.Json;

namespace Api.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = new ApiResponseDto<object>
        {
            Success = false,
            Data = null,
            Error = GetErrorMessage(exception)
        };

        var statusCode = GetStatusCode(exception);
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        var jsonResponse = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(jsonResponse);
    }

    private static string GetErrorMessage(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException => "ไม่มีสิทธิ์เข้าถึงข้อมูลนี้",
            ArgumentException => "ข้อมูลที่ส่งมาไม่ถูกต้อง",
            KeyNotFoundException => "ไม่พบข้อมูลที่ต้องการ",
            InvalidOperationException => exception.Message,
            TimeoutException => "การดำเนินการใช้เวลานานเกินไป กรุณาลองใหม่อีกครั้ง",
            _ => "เกิดข้อผิดพลาดภายในระบบ กรุณาติดต่อผู้ดูแลระบบ"
        };
    }

    private static HttpStatusCode GetStatusCode(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            ArgumentException => HttpStatusCode.BadRequest,
            KeyNotFoundException => HttpStatusCode.NotFound,
            InvalidOperationException => HttpStatusCode.BadRequest,
            TimeoutException => HttpStatusCode.RequestTimeout,
            _ => HttpStatusCode.InternalServerError
        };
    }
}