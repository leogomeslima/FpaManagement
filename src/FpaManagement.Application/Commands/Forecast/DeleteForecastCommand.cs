using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Forecast;

[Authorize(Permissions = "DeleteForecast")]
public class DeleteForecastCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}

public class DeleteForecastCommandHandler : IRequestHandler<DeleteForecastCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteForecastCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteForecastCommand request, CancellationToken cancellationToken)
    {
        var forecast = await _context.Forecasts
            .FirstOrDefaultAsync(f => f.Id == request.Id && !f.IsDeleted, cancellationToken);

        if (forecast == null)
            throw new NotFoundException(nameof(Forecast), request.Id);

        forecast.IsDeleted = true;
        forecast.DeletedAt = DateTime.UtcNow;
        forecast.DeletedBy = _currentUserService.UserEmail ?? "system";

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Previsão excluída com sucesso");
    }
}
