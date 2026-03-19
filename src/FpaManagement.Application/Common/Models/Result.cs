namespace FpaManagement.Application.Common.Models;

public class Result
{
    public bool Succeeded { get; set; }
    public string[] Errors { get; set; } = Array.Empty<string>();
    public string? Message { get; set; }

    public static Result Success()
    {
        return new Result { Succeeded = true };
    }

    public static Result Success(string message)
    {
        return new Result { Succeeded = true, Message = message };
    }

    public static Result Failure(params string[] errors)
    {
        return new Result { Succeeded = false, Errors = errors };
    }

    public static Result Failure(string message, params string[] errors)
    {
        var allErrors = new List<string> { message };
        allErrors.AddRange(errors);
        return new Result { Succeeded = false, Errors = allErrors.ToArray(), Message = message };
    }
}

public class Result<T> : Result
{
    public T? Data { get; set; }

    public static Result<T> Success(T data)
    {
        return new Result<T> { Succeeded = true, Data = data };
    }

    public static Result<T> Success(T data, string message)
    {
        return new Result<T> { Succeeded = true, Data = data, Message = message };
    }

    public new static Result<T> Failure(params string[] errors)
    {
        return new Result<T> { Succeeded = false, Errors = errors };
    }

    public new static Result<T> Failure(string message, params string[] errors)
    {
        var allErrors = new List<string> { message };
        allErrors.AddRange(errors);
        return new Result<T> { Succeeded = false, Errors = allErrors.ToArray(), Message = message };
    }
}
