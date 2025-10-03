using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Test endpoints สำหรับตรวจสอบการทำงานของ API
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class TestController : BaseController
{
    /// <summary>
    /// Simple test endpoint
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(object), 200)]
    public ActionResult<object> GetTest()
    {
        return Ok(new
        {
            message = "Test endpoint working!",
            timestamp = DateTime.UtcNow,
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            version = "1.0.0"
        });
    }

    /// <summary>
    /// Get API status
    /// </summary>
    [HttpGet("status")]
    [ProducesResponseType(typeof(object), 200)]
    public ActionResult<object> GetStatus()
    {
        return Ok(new
        {
            status = "healthy",
            database = "connected",
            features = new[]
            {
                "authentication",
                "user_management", 
                "posts",
                "comments",
                "reports"
            }
        });
    }

    /// <summary>
    /// Echo endpoint for testing POST requests
    /// </summary>
    [HttpPost("echo")]
    [ProducesResponseType(typeof(object), 200)]
    public ActionResult<object> Echo([FromBody] object data)
    {
        return Ok(new
        {
            received = data,
            timestamp = DateTime.UtcNow,
            echo = "Data received successfully"
        });
    }
}