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
public class AddBudgetItemCommand : IRequest<Result<BudgetItemDto>>
{
    public CreateBudgetItemDto Data { get; set; } = null!;
}

public class AddBudgetItemCommandHandler : IRequestHandler<AddBudgetItemCommand, Result<BudgetItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public AddBudgetItemCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Result<BudgetItemDto>> Handle(AddBudgetItemCommand request, CancellationToken cancellationToken)
    {
        // Verificar se a versão do orçamento existe
        var version = await _context.BudgetVersions
            .Include(v => v.Budget)
            .FirstOrDefaultAsync(v => v.Id == request.Data.BudgetVersionId && !v.IsDeleted, cancellationToken);

        if (version == null)
        {
            throw new NotFoundException(nameof(BudgetVersion), request.Data.BudgetVersionId);
        }

        // Verificar se o orçamento pode ser editado
        if (version.Budget.Status != Domain.Enums.BudgetStatus.Draft)
        {
            return Result<BudgetItemDto>.Failure("Não é possível adicionar itens a um orçamento que não está em rascunho");
        }

        // Criar item de orçamento
        var plannedAmount = new Money(request.Data.PlannedAmount, request.Data.Currency);

        var item = new BudgetItem(
            request.Data.BudgetVersionId,
            request.Data.Category,
            request.Data.Description,
            plannedAmount,
            request.Data.SubCategory,
            request.Data.CostCenterId,
            accountCode: request.Data.AccountCode);

        item.CreatedBy = _currentUserService.UserEmail ?? "system";

        version.AddItem(item);
        version.UpdatedBy = _currentUserService.UserEmail ?? "system";
        version.UpdatedAt = DateTime.UtcNow;

        // Atualizar total do orçamento
        version.Budget.UpdateTotalAmount();

        await _context.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<BudgetItemDto>(item);
        return Result<BudgetItemDto>.Success(result, "Item adicionado com sucesso");
    }
}
