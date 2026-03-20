using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.CostCenter;

[Authorize(Permissions = "DeleteCostCenter")]
public class DeleteCostCenterCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}

public class DeleteCostCenterCommandHandler : IRequestHandler<DeleteCostCenterCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteCostCenterCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteCostCenterCommand request, CancellationToken cancellationToken)
    {
        var costCenter = await _context.CostCenters
            .Include(cc => cc.BudgetItems)
            .Include(cc => cc.CashFlowEntries)
            .Include(cc => cc.Revenues)
            .Include(cc => cc.Expenses)
            .FirstOrDefaultAsync(cc => cc.Id == request.Id && !cc.IsDeleted, cancellationToken);
        if (costCenter == null)
            throw new NotFoundException(nameof(Domain.Entities.CostCenter), request.Id);

        if (costCenter.BudgetItems.Any(b => !b.IsDeleted) ||
            costCenter.CashFlowEntries.Any(c => !c.IsDeleted) ||
            costCenter.Revenues.Any(r => !r.IsDeleted) ||
            costCenter.Expenses.Any(e => !e.IsDeleted))
            return Result.Failure("Não é possível excluir um centro de custo com lançamentos ou orçamentos vinculados.");

        costCenter.IsDeleted = true;
        costCenter.DeletedAt = DateTime.UtcNow;
        costCenter.DeletedBy = _currentUserService.UserEmail ?? "system";

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("Centro de custo excluído com sucesso.");
    }
}
