using System.Net;
using System.Text.Json;
using FpaManagement.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FpaManagement.Infrastructure.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            Status = "error",
            Message = "Ocorreu um erro ao processar sua solicitação.",
            Errors = Array.Empty<string>(),
            Details = string.Empty
        };

        switch (exception)
        {
            case ValidationException validationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    Status = "error",
                    Message = "Erro de validação",
                    Errors = validationException.Errors.Select(e => e.ErrorMessage).ToArray(),
                    Details = string.Empty
                };
                _logger.LogWarning(validationException, "Validation error");
                break;

            case NotFoundException notFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new
                {
                    Status = "error",
                    Message = notFoundException.Message,
                    Errors = Array.Empty<string>(),
                    Details = string.Empty
                };
                _logger.LogInformation(notFoundException, "Resource not found");
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = new
                {
                    Status = "error",
                    Message = "Não autorizado",
                    Errors = Array.Empty<string>(),
                    Details = string.Empty
                };
                _logger.LogWarning(exception, "Unauthorized access attempt");
                break;

            case ForbiddenAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                response = new
                {
                    Status = "error",
                    Message = "Acesso negado",
                    Errors = Array.Empty<string>(),
                    Details = string.Empty
                };
                _logger.LogWarning(exception, "Forbidden access attempt");
                break;

            case BusinessRuleException businessException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    Status = "error",
                    Message = businessException.Message,
                    Code = businessException.Code,
                    Errors = Array.Empty<string>(),
                    Details = string.Empty
                };
                _logger.LogWarning(businessException, "Business rule violation");
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new
                {
                    Status = "error",
                    Message = "Ocorreu um erro interno no servidor.",
                    Errors = Array.Empty<string>(),
                    Details = exception.Message
                };
                _logger.LogError(exception, "Unhandled exception");
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
