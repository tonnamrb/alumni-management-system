using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Helpers;

/// <summary>
/// Helper class for creating standardized API responses
/// </summary>
public static class ApiResponseHelper
{
    public static IActionResult Success<T>(T data, string? message = null)
    {
        return new OkObjectResult(ApiResponseDto<T>.CreateSuccess(data, message));
    }

    public static IActionResult BadRequest(string message, object? error = null)
    {
        return new BadRequestObjectResult(ApiResponseDto<object>.CreateFailure(message, error?.ToString()));
    }

    public static IActionResult NotFound(string message)
    {
        return new NotFoundObjectResult(ApiResponseDto<object>.CreateFailure(message));
    }

    public static IActionResult InternalServerError(string message)
    {
        return new ObjectResult(ApiResponseDto<object>.CreateFailure(message)) { StatusCode = 500 };
    }
}