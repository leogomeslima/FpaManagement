using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FpaManagement.API.Middleware;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

    public ApiExceptionFilterAttribute()
    {
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
        {
            { typeof(ValidationException), HandleValidationException },
            { typeof(NotFoundException), HandleNotFoundException },
            { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
            { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
            { typeof(BusinessRuleException), HandleBusinessRuleException }
        };
    }

    public override void OnException(ExceptionContext context)
    {
        HandleException(context);
        base.OnException(context);
    }

    private void HandleException(ExceptionContext context)
    {
        var type = context.Exception.GetType();

        if (_exceptionHandlers.TryGetValue(type, out var handler))
        {
            handler.Invoke(context);
            return;
        }

        if (!context.ModelState.IsValid)
        {
            HandleInvalidModelStateException(context);
            return;
        }

        HandleUnknownException(context);
    }

    private void HandleValidationException(ExceptionContext context)
    {
        var exception = (ValidationException)context.Exception;

        var result = Result.Failure(exception.Message, exception.Errors.Values.SelectMany(v => v).ToArray());

        context.Result = new BadRequestObjectResult(result);
        context.ExceptionHandled = true;
    }

    private void HandleNotFoundException(ExceptionContext context)
    {
        var exception = (NotFoundException)context.Exception;

        var result = Result.Failure(exception.Message);

        context.Result = new NotFoundObjectResult(result);
        context.ExceptionHandled = true;
    }

    private void HandleUnauthorizedAccessException(ExceptionContext context)
    {
        var result = Result.Failure("Não autorizado");

        context.Result = new UnauthorizedObjectResult(result);
        context.ExceptionHandled = true;
    }

    private void HandleForbiddenAccessException(ExceptionContext context)
    {
        var result = Result.Failure("Acesso negado");

        context.Result = new ForbidResult();
        context.ExceptionHandled = true;
    }

    private void HandleBusinessRuleException(ExceptionContext context)
    {
        var exception = (BusinessRuleException)context.Exception;

        var result = Result.Failure(exception.Message);

        context.Result = new BadRequestObjectResult(result);
        context.ExceptionHandled = true;
    }

    private void HandleInvalidModelStateException(ExceptionContext context)
    {
        var errors = context.ModelState
            .Where(ms => ms.Value?.Errors.Count > 0)
            .SelectMany(ms => ms.Value!.Errors.Select(e => e.ErrorMessage))
            .ToArray();

        var result = Result.Failure("Erro de validação", errors);

        context.Result = new BadRequestObjectResult(result);
        context.ExceptionHandled = true;
    }

    private void HandleUnknownException(ExceptionContext context)
    {
        var result = Result.Failure("Ocorreu um erro ao processar sua solicitação.");

        context.Result = new ObjectResult(result)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
        context.ExceptionHandled = true;
    }
}
