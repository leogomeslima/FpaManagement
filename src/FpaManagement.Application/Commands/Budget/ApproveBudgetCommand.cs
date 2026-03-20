using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Budget;

[Authorize(Permissions = "ApproveBudget")]
public class ApproveBudgetCommand : IRequest<Result>
{
    public Guid BudgetId { get; set; }
    public string? Comments { get; set; }
}

public class ApproveBudgetCommandHandler : IRequestHandler<ApproveBudgetCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ApproveBudgetCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(ApproveBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await _context.Budgets
            .Include(b => b.Versions)
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
            budget.Approve(_currentUserService.UserId.Value.ToString(), request.Comments);

            // Aprovar a versão atual
            var currentVersion = budget.Versions.FirstOrDefault(v => v.IsCurrent);
            if (currentVersion != null)
            {
                currentVersion.Approve(_currentUserService.UserEmail ?? "system");
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success("Orçamento aprovado com sucesso");
        }
        catch (Domain.Exceptions.InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
