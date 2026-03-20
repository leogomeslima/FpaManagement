using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Budget;

[Authorize(Permissions = "EditBudget")]
public class DeleteBudgetItemCommand : IRequest<Result>
{
    public Guid ItemId { get; set; }
}

public class DeleteBudgetItemCommandHandler : IRequestHandler<DeleteBudgetItemCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteBudgetItemCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteBudgetItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.BudgetItems
            .Include(i => i.BudgetVersion)
                .ThenInclude(v => v.Budget)
            .FirstOrDefaultAsync(i => i.Id == request.ItemId && !i.IsDeleted, cancellationToken);
        if (item == null)
            throw new NotFoundException(nameof(BudgetItem), request.ItemId);

        if (item.BudgetVersion.Budget.Status != Domain.Enums.BudgetStatus.Draft)
            return Result.Failure("Não é possível excluir itens de um orçamento que não está em rascunho.");

        item.IsDeleted = true;
        item.DeletedAt = DateTime.UtcNow;
        item.DeletedBy = _currentUserService.UserEmail ?? "system";

        item.BudgetVersion.Budget.UpdateTotalAmount();

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("Item excluído.");
    }
}
