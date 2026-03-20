using AutoMapper;
using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Forecast;
using FpaManagement.Domain.Entities;
using FpaManagement.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Forecast;

[Authorize(Permissions = "EditForecast")]
public class AddForecastItemCommand : IRequest<Result<ForecastItemDto>>
{
    public CreateForecastItemDto Data { get; set; } = null!;
}

public class AddForecastItemCommandHandler : IRequestHandler<AddForecastItemCommand, Result<ForecastItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public AddForecastItemCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ForecastItemDto>> Handle(AddForecastItemCommand request, CancellationToken cancellationToken)
    {
        var version = await _context.ForecastVersions
            .Include(v => v.Forecast)
            .FirstOrDefaultAsync(v => v.Id == request.Data.ForecastVersionId && !v.IsDeleted, cancellationToken);

        if (version == null)
            throw new NotFoundException(nameof(ForecastVersion), request.Data.ForecastVersionId);

        if (version.Forecast.Status != Domain.Enums.ForecastStatus.Draft)
            return Result<ForecastItemDto>.Failure("Não é possível adicionar itens a uma previsão que não está em rascunho");

        var amount = new Money(request.Data.Amount, request.Data.Currency);
        var item = new ForecastItem(
            request.Data.ForecastVersionId,
            request.Data.Category,
            request.Data.Description,
            amount,
            request.Data.SubCategory,
            request.Data.CostCenterId,
            request.Data.AccountCode);

        item.CreatedBy = _currentUserService.UserEmail ?? "system";

        version.AddItem(item);
        var newTotal = version.CalculateTotal();
        version.Forecast.UpdateTotalAmount(newTotal);

        await _context.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<ForecastItemDto>(item);
        return Result<ForecastItemDto>.Success(result, "Item adicionado com sucesso");
    }
}
