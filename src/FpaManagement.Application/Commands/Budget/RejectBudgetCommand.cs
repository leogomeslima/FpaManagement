using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Budget;

[Authorize(Permissions = "ApproveBudget")]
public class RejectBudgetCommand : IRequest<Result>
{
    public Guid BudgetId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class RejectBudgetCommandHandler : IRequestHandler<RejectBudgetCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RejectBudgetCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(RejectBudgetCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return Result.Failure("O motivo da rejeição é obrigatório");
        }

        var budget = await _context.Budgets
            .FirstOrDefaultAsync(b => b.Id == request.BudgetId && !b.IsDeleted, cancellationToken);

        if (budget == null)
        {
            throw new NotFoundException(nameof(Budget), request.BudgetId);
        }

        if (_currentUserService.UserId == null)
        {
            return Result.Failure("Usuário não autenticado");
        }

        try
        {
            budget.Reject(_currentUserService.UserId.Value.ToString(), request.Reason);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success("Orçamento rejeitado");
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
