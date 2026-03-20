using AutoMapper;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.CashFlow;
using FpaManagement.Domain.Entities;
using FpaManagement.Domain.ValueObjects;
using MediatR;

namespace FpaManagement.Application.Commands.CashFlow;

[Authorize(Permissions = "CreateCashFlow")]
public class CreateCashFlowEntryCommand : IRequest<Result<CashFlowEntryDto>>
{
    public CreateCashFlowEntryDto Data { get; set; } = null!;
}

public class CreateCashFlowEntryCommandHandler : IRequestHandler<CreateCashFlowEntryCommand, Result<CashFlowEntryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CreateCashFlowEntryCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CashFlowEntryDto>> Handle(CreateCashFlowEntryCommand request, CancellationToken cancellationToken)
    {
        var amount = new Money(request.Data.Amount, request.Data.Currency);
        var entry = new CashFlowEntry(
            request.Data.Date,
            request.Data.Description,
            amount,
            request.Data.Type,
            request.Data.CategoryId,
            request.Data.CostCenterId,
            request.Data.DocumentNumber);

        entry.CreatedBy = _currentUserService.UserEmail ?? "system";

        _context.CashFlowEntries.Add(entry);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<CashFlowEntryDto>(entry);
        return Result<CashFlowEntryDto>.Success(dto, "Lançamento criado com sucesso.");
    }
}
