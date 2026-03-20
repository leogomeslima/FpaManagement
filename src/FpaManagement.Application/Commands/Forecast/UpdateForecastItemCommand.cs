using AutoMapper;
using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Forecast;
using FpaManagement.Domain.Entities;
using FpaManagement.Domain.Exceptions;
using FpaManagement.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Forecast;

[Authorize(Permissions = "EditForecast")]
public class UpdateForecastItemCommand : IRequest<Result<ForecastItemDto>>
{
    public Guid Id { get; set; }
    public UpdateForecastItemDto Data { get; set; } = null!;
}

public class UpdateForecastItemCommandHandler : IRequestHandler<UpdateForecastItemCommand, Result<ForecastItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UpdateForecastItemCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ForecastItemDto>> Handle(UpdateForecastItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.ForecastItems
            .Include(i => i.ForecastVersion)
            .ThenInclude(v => v.Forecast)
            .FirstOrDefaultAsync(i => i.Id == request.Id && !i.IsDeleted, cancellationToken);

        if (item == null)
            throw new NotFoundException(nameof(ForecastItem), request.Id);

        if (item.ForecastVersion.Forecast.Status != Domain.Enums.ForecastStatus.Draft)
            return Result<ForecastItemDto>.Failure("Não é possível editar itens de uma previsão que não está em rascunho");

        // Atualizar campos
        item.Update(
            request.Data.Category,
            request.Data.SubCategory,
            request.Data.Description,
            new Money(request.Data.Amount, request.Data.Currency),
            request.Data.CostCenterId,
            request.Data.AccountCode);

        item.UpdatedBy = _currentUserService.UserEmail ?? "system";

        // Recalcular total da versão
        item.ForecastVersion.Forecast.TotalAmount = item.ForecastVersion.CalculateTotal();

        await _context.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<ForecastItemDto>(item);
        return Result<ForecastItemDto>.Success(result, "Item atualizado com sucesso");
    }
}
