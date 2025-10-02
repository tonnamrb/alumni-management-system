using System.Diagnostics;
using System.Text;

namespace Api.Middleware;

/// <summary>
/// Request/Response logging middleware
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // ข้ามการ log สำหรับ health check และ static files
        if (ShouldSkipLogging(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString("N")[..8];

        // Log request
        await LogRequestAsync(context, requestId);

        // เก็บ response stream เดิม
        var originalResponseBodyStream = context.Response.Body;

        try
        {
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            // ดำเนินการ request
            await _next(context);

            stopwatch.Stop();

            // Log response
            await LogResponseAsync(context, requestId, stopwatch.ElapsedMilliseconds, responseBodyStream);

            // Copy response กลับไปยัง stream เดิม
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalResponseBodyStream);
        }
        finally
        {
            context.Response.Body = originalResponseBodyStream;
            stopwatch.Stop();
        }
    }

    private async Task LogRequestAsync(HttpContext context, string requestId)
    {
        var request = context.Request;
        
        var logData = new
        {
            RequestId = requestId,
            Method = request.Method,
            Path = request.Path.Value,
            QueryString = request.QueryString.Value,
            Headers = GetSafeHeaders(request.Headers),
            UserAgent = request.Headers.UserAgent.ToString(),
            IPAddress = GetClientIpAddress(context),
            UserId = GetUserId(context),
            Timestamp = DateTime.UtcNow
        };

        _logger.LogInformation("HTTP Request: {@RequestData}", logData);

        // Log request body สำหรับ POST/PUT requests
        if (ShouldLogRequestBody(request))
        {
            var requestBody = await ReadRequestBodyAsync(request);
            if (!string.IsNullOrEmpty(requestBody))
            {
                _logger.LogInformation("Request Body for {RequestId}: {RequestBody}", 
                    requestId, requestBody);
            }
        }
    }

    private async Task LogResponseAsync(HttpContext context, string requestId, long elapsedMs, MemoryStream responseBodyStream)
    {
        var response = context.Response;
        
        var logData = new
        {
            RequestId = requestId,
            StatusCode = response.StatusCode,
            ContentType = response.ContentType,
            ContentLength = response.ContentLength ?? responseBodyStream.Length,
            Headers = GetSafeResponseHeaders(response.Headers),
            ElapsedMilliseconds = elapsedMs,
            Timestamp = DateTime.UtcNow
        };

        var logLevel = response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;
        _logger.Log(logLevel, "HTTP Response: {@ResponseData}", logData);

        // Log response body สำหรับ errors
        if (response.StatusCode >= 400 && ShouldLogResponseBody(response))
        {
            var responseBody = await ReadResponseBodyAsync(responseBodyStream);
            if (!string.IsNullOrEmpty(responseBody))
            {
                _logger.LogWarning("Response Body for {RequestId}: {ResponseBody}", 
                    requestId, responseBody);
            }
        }
    }

    private static bool ShouldSkipLogging(string path)
    {
        var skipPaths = new[]
        {
            "/health",
            "/swagger",
            "/favicon.ico",
            "/_framework",
            "/css",
            "/js",
            "/lib"
        };

        return skipPaths.Any(skipPath => path.StartsWith(skipPath, StringComparison.OrdinalIgnoreCase));
    }

    private static bool ShouldLogRequestBody(HttpRequest request)
    {
        if (!HttpMethods.IsPost(request.Method) && !HttpMethods.IsPut(request.Method))
            return false;

        var contentType = request.ContentType?.ToLowerInvariant();
        return contentType != null && 
               (contentType.Contains("application/json") || contentType.Contains("application/xml"));
    }

    private static bool ShouldLogResponseBody(HttpResponse response)
    {
        var contentType = response.ContentType?.ToLowerInvariant();
        return contentType != null && 
               (contentType.Contains("application/json") || contentType.Contains("application/xml"));
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        try
        {
            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength ?? 0)];
            await request.Body.ReadAsync(buffer);
            request.Body.Seek(0, SeekOrigin.Begin);
            
            var bodyText = Encoding.UTF8.GetString(buffer);
            return SanitizeRequestBody(bodyText);
        }
        catch
        {
            return string.Empty;
        }
    }

    private static async Task<string> ReadResponseBodyAsync(MemoryStream responseBodyStream)
    {
        try
        {
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(responseBodyStream, Encoding.UTF8, leaveOpen: true);
            var responseBody = await reader.ReadToEndAsync();
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            
            return responseBody;
        }
        catch
        {
            return string.Empty;
        }
    }

    private static Dictionary<string, string> GetSafeHeaders(IHeaderDictionary headers)
    {
        var safeHeaders = new Dictionary<string, string>();
        var sensitiveHeaders = new[] { "authorization", "cookie", "x-api-key" };

        foreach (var header in headers)
        {
            var key = header.Key.ToLowerInvariant();
            var value = sensitiveHeaders.Contains(key) ? "***REDACTED***" : header.Value.ToString();
            safeHeaders[header.Key] = value;
        }

        return safeHeaders;
    }

    private static Dictionary<string, string> GetSafeResponseHeaders(IHeaderDictionary headers)
    {
        var safeHeaders = new Dictionary<string, string>();
        var sensitiveHeaders = new[] { "set-cookie" };

        foreach (var header in headers)
        {
            var key = header.Key.ToLowerInvariant();
            var value = sensitiveHeaders.Contains(key) ? "***REDACTED***" : header.Value.ToString();
            safeHeaders[header.Key] = value;
        }

        return safeHeaders;
    }

    private static string GetClientIpAddress(HttpContext context)
    {
        // ตรวจสอบ headers ต่างๆ สำหรับ IP address
        var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(ipAddress))
            ipAddress = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(ipAddress))
            ipAddress = context.Connection.RemoteIpAddress?.ToString();

        return ipAddress ?? "Unknown";
    }

    private static string? GetUserId(HttpContext context)
    {
        return context.User?.Claims
            ?.FirstOrDefault(c => c.Type == "user_id" || c.Type == "sub")
            ?.Value;
    }

    private static string SanitizeRequestBody(string body)
    {
        if (string.IsNullOrEmpty(body))
            return body;

        // ลบข้อมูล sensitive ออกจาก request body
        var sensitiveFields = new[] { "password", "token", "secret", "key" };
        
        foreach (var field in sensitiveFields)
        {
            var pattern = $@"""{field}"":\s*""[^""]*""";
            body = System.Text.RegularExpressions.Regex.Replace(
                body, pattern, $@"""{field}"":""***REDACTED***""", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        return body;
    }
}