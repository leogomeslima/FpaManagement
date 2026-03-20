using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Domain.Entities;
using FpaManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Forecast;

[Authorize(Permissions = "EditForecast")]
public class DeleteForecastItemCommand : IRequest<Result>
{
    public Guid ItemId { get; set; }
}

public class DeleteForecastItemCommandHandler : IRequestHandler<DeleteForecastItemCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteForecastItemCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteForecastItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.ForecastItems
            .Include(i => i.ForecastVersion)
            .ThenInclude(v => v.Forecast)
            .FirstOrDefaultAsync(i => i.Id == request.ItemId && !i.IsDeleted, cancellationToken);

        if (item == null)
            throw new NotFoundException(nameof(ForecastItem), request.ItemId);

        if (item.ForecastVersion.Forecast.Status != Domain.Enums.ForecastStatus.Draft)
            return Result.Failure("Não é possível excluir itens de uma previsão que não está em rascunho");

        item.IsDeleted = true;
        item.DeletedAt = DateTime.UtcNow;
        item.DeletedBy = _currentUserService.UserEmail ?? "system";

        // Recalcular total da versão
        item.ForecastVersion.Forecast.TotalAmount = item.ForecastVersion.CalculateTotal();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Item excluído com sucesso");
    }
}
