namespace Domain.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, int id) 
        : base($"{entityName} with ID {id} was not found.")
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }
}