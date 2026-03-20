using AutoMapper;
using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.CashFlow;
using FpaManagement.Domain.Entities;
using FpaManagement.Domain.Exceptions;
using FpaManagement.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.CashFlow;

[Authorize(Permissions = "EditCashFlow")]
public class UpdateCashFlowEntryCommand : IRequest<Result<CashFlowEntryDto>>
{
    public Guid Id { get; set; }

    public UpdateCashFlowEntryDto Data { get; set; } = null!;
}

public class UpdateCashFlowEntryCommandHandler : IRequestHandler<UpdateCashFlowEntryCommand, Result<CashFlowEntryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCashFlowEntryCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CashFlowEntryDto>> Handle(
        UpdateCashFlowEntryCommand request,
        CancellationToken cancellationToken)
    {
        var entry = await _context.CashFlowEntries
            .FirstOrDefaultAsync(e => e.Id == request.Id && !e.IsDeleted, cancellationToken);

        if (entry == null)
            throw new NotFoundException(nameof(CashFlowEntry), request.Id);

        entry.Update(
            request.Data.Date,
            request.Data.Description,
            new Money(request.Data.Amount, request.Data.Currency),
            request.Data.Type,
            request.Data.CategoryId,
            request.Data.CostCenterId,
            request.Data.DocumentNumber);

        entry.UpdatedBy = _currentUserService.UserEmail ?? "system";

        await _context.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<CashFlowEntryDto>(entry);

        return Result<CashFlowEntryDto>.Success(dto, "Lançamento atualizado.");
    }
}
