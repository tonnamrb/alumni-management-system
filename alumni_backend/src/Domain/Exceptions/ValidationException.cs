namespace Domain.Exceptions;

public class ValidationException : DomainException
{
    public ValidationException(string message) : base(message)
    {
    }

    public ValidationException(string property, string message) 
        : base($"Validation failed for {property}: {message}")
    {
    }
}