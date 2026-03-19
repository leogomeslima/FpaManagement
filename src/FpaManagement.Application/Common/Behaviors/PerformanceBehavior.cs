using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using FpaManagement.Application.Common.Interfaces;

namespace FpaManagement.Application.Common.Behaviors;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;
    private readonly ICurrentUserService _currentUserService;

    public PerformanceBehavior(ILogger<TRequest> logger, ICurrentUserService currentUserService)
    {
        _timer = new Stopwatch();
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500) // Log if > 500ms
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId?.ToString() ?? "Anonymous";
            var userEmail = _currentUserService.UserEmail ?? "Anonymous";

            _logger.LogWarning("FPA Long Running Request: {Name} ({ElapsedMilliseconds} ms) {@UserId} {@UserEmail} {@Request}",
                requestName, elapsedMilliseconds, userId, userEmail, request);
        }

        return response;
    }
}
