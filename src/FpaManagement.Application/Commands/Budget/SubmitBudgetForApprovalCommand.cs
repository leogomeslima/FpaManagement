using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Budget;

[Authorize(Permissions = "ApproveBudget")]
public class SubmitBudgetForApprovalCommand : IRequest<Result>
{
    public Guid BudgetId { get; set; }
}

public class SubmitBudgetForApprovalCommandHandler : IRequestHandler<SubmitBudgetForApprovalCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public SubmitBudgetForApprovalCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(SubmitBudgetForApprovalCommand request, CancellationToken cancellationToken)
    {
        var budget = await _context.Budgets
            .Include(b => b.Versions)
            .ThenInclude(v => v.Items)
            .FirstOrDefaultAsync(b => b.Id == request.BudgetId && !b.IsDeleted, cancellationToken);

        if (budget == null)
        {
            throw new NotFoundException(nameof(Budget), request.BudgetId);
        }

        // Verificar se há pelo menos um item no orçamento
        var currentVersion = budget.Versions.FirstOrDefault(v => v.IsCurrent);
        if (currentVersion == null || !currentVersion.Items.Any())
        {
            return Result.Failure("O orçamento deve ter pelo menos um item para ser submetido à aprovação");
        }

        try
        {
            budget.SubmitForApproval();
            budget.UpdatedBy = _currentUserService.UserEmail ?? "system";
            budget.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success("Orçamento submetido para aprovação com sucesso");
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
