using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Forecast;

[Authorize(Permissions = "ApproveForecast")]
public class RejectForecastCommand : IRequest<Result>
{
    public Guid ForecastId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class RejectForecastCommandHandler : IRequestHandler<RejectForecastCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RejectForecastCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(RejectForecastCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
            return Result.Failure("O motivo da rejeição é obrigatório");

        var forecast = await _context.Forecasts
            .FirstOrDefaultAsync(f => f.Id == request.ForecastId && !f.IsDeleted, cancellationToken);

        if (forecast == null)
            throw new NotFoundException(nameof(Forecast), request.ForecastId);

        try
        {
            forecast.Reject();
            forecast.UpdatedBy = _currentUserService.UserEmail ?? "system";
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success("Previsão rejeitada");
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
