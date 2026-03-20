using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Domain.Entities;
using FpaManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.CashFlow;

[Authorize(Permissions = "EditCashFlow")]
public class ReconcileCashFlowEntryCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}

public class ReconcileCashFlowEntryCommandHandler : IRequestHandler<ReconcileCashFlowEntryCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ReconcileCashFlowEntryCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(ReconcileCashFlowEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await _context.CashFlowEntries
            .FirstOrDefaultAsync(e => e.Id == request.Id && !e.IsDeleted, cancellationToken);
        if (entry == null)
            throw new NotFoundException(nameof(CashFlowEntry), request.Id);

        entry.Reconcile();
        entry.UpdatedBy = _currentUserService.UserEmail ?? "system";

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("Lançamento reconciliado.");
    }
}
