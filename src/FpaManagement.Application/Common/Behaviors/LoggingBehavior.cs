using MediatR;
using Microsoft.Extensions.Logging;
using FpaManagement.Application.Common.Interfaces;

namespace FpaManagement.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.UserId?.ToString() ?? "Anonymous";
        var userEmail = _currentUserService.UserEmail ?? "Anonymous";

        _logger.LogInformation("FPA Request: {Name} {@UserId} {@UserEmail} {@Request}",
            requestName, userId, userEmail, request);

        try
        {
            var response = await next();

            _logger.LogInformation("FPA Response: {Name} {@UserId} {@Response}",
                requestName, userId, response);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "FPA Request Failed: {Name} {@UserId} {@Error}",
                requestName, userId, ex.Message);
            throw;
        }
    }
}
