using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Forecast;

[Authorize(Permissions = "EditForecast")]
public class SubmitForecastCommand : IRequest<Result>
{
    public Guid ForecastId { get; set; }
}

public class SubmitForecastCommandHandler : IRequestHandler<SubmitForecastCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public SubmitForecastCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(SubmitForecastCommand request, CancellationToken cancellationToken)
    {
        var forecast = await _context.Forecasts
            .Include(f => f.Versions)
            .ThenInclude(v => v.Items)
            .FirstOrDefaultAsync(f => f.Id == request.ForecastId && !f.IsDeleted, cancellationToken);

        if (forecast == null)
            throw new NotFoundException(nameof(Forecast), request.ForecastId);

        var currentVersion = forecast.Versions.FirstOrDefault(v => v.IsCurrent);
        if (currentVersion == null || !currentVersion.Items.Any())
            return Result.Failure("A previsão deve ter pelo menos um item para ser submetida");

        try
        {
            forecast.Submit();
            forecast.UpdatedBy = _currentUserService.UserEmail ?? "system";
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success("Previsão submetida com sucesso");
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
