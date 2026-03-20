using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Forecast;

[Authorize(Permissions = "ApproveForecast")]
public class ApproveForecastCommand : IRequest<Result>
{
    public Guid ForecastId { get; set; }
}

public class ApproveForecastCommandHandler : IRequestHandler<ApproveForecastCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ApproveForecastCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(ApproveForecastCommand request, CancellationToken cancellationToken)
    {
        var forecast = await _context.Forecasts
            .Include(f => f.Versions)
            .FirstOrDefaultAsync(f => f.Id == request.ForecastId && !f.IsDeleted, cancellationToken);

        if (forecast == null)
            throw new NotFoundException(nameof(Forecast), request.ForecastId);

        try
        {
            forecast.Approve();
            var currentVersion = forecast.Versions.FirstOrDefault(v => v.IsCurrent);
            if (currentVersion != null)
                currentVersion.Approve(_currentUserService.UserEmail ?? "system");

            forecast.UpdatedBy = _currentUserService.UserEmail ?? "system";
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success("Previsão aprovada com sucesso");
        }
        catch (System.InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
