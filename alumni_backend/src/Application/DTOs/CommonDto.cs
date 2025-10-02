namespace Application.DTOs;

public class PaginatedResultDto<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public string? Message { get; set; }

    public static ApiResponseDto<T> CreateSuccess(T data, string? message = null)
    {
        return new ApiResponseDto<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponseDto<T> CreateFailure(string message, string? error = null)
    {
        return new ApiResponseDto<T>
        {
            Success = false,
            Message = message,
            Error = error
        };
    }
}

public class ValidationErrorDto
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class ValidationResultDto
{
    public bool IsValid { get; set; }
    public List<ValidationErrorDto> Errors { get; set; } = new();
}