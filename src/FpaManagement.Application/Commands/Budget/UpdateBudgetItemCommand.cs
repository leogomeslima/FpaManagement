using AutoMapper;
using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Budget;
using FpaManagement.Domain.Entities;
using FpaManagement.Domain.Exceptions;
using FpaManagement.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Budget;

[Authorize(Permissions = "EditBudget")]
public class UpdateBudgetItemCommand : IRequest<Result<BudgetItemDto>>
{
    public Guid ItemId { get; set; }
    public UpdateBudgetItemDto Data { get; set; } = null!;
}

public class UpdateBudgetItemCommandHandler : IRequestHandler<UpdateBudgetItemCommand, Result<BudgetItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UpdateBudgetItemCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Result<BudgetItemDto>> Handle(UpdateBudgetItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.BudgetItems
            .Include(i => i.BudgetVersion)
                .ThenInclude(v => v.Budget)
            .FirstOrDefaultAsync(i => i.Id == request.ItemId && !i.IsDeleted, cancellationToken);
        if (item == null)
            throw new NotFoundException(nameof(BudgetItem), request.ItemId);

        if (item.BudgetVersion.Budget.Status != Domain.Enums.BudgetStatus.Draft)
            return Result<BudgetItemDto>.Failure("Não é possível editar itens de um orçamento que não está em rascunho.");

        var newAmount = new Money(request.Data.PlannedAmount, request.Data.Currency);
        item.UpdatePlannedAmount(newAmount);
        item.UpdatedBy = _currentUserService.UserEmail ?? "system";

        item.BudgetVersion.Budget.UpdateTotalAmount();

        await _context.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<BudgetItemDto>(item);
        return Result<BudgetItemDto>.Success(dto, "Item atualizado.");
    }
}
