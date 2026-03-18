namespace FpaManagement.Domain.Exceptions;

public class DomainException : Exception
{
    public string Code
    {
        get;
    }

    public DomainException(string message, string code = "DOMAIN_ERROR") : base(message)
    {
        Code = code;
    }

    public DomainException(string message, Exception innerException, string code = "DOMAIN_ERROR")
        : base(message, innerException)
    {
        Code = code;
    }
}

public class BusinessRuleException : DomainException
{
    public BusinessRuleException(string message, string code = "BUSINESS_RULE_VIOLATION")
        : base(message, code)
    {
    }
}

public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object key)
        : base($"Entity '{entityName}' with key '{key}' was not found.", "ENTITY_NOT_FOUND")
    {
    }
}

public class InvalidOperationException : DomainException
{
    public InvalidOperationException(string message)
        : base(message, "INVALID_OPERATION")
    {
    }
}
