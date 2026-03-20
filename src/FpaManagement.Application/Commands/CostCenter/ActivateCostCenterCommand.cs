using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.CostCenter;

[Authorize(Permissions = "EditCostCenter")]
public class ActivateCostCenterCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}

public class ActivateCostCenterCommandHandler : IRequestHandler<ActivateCostCenterCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ActivateCostCenterCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(ActivateCostCenterCommand request, CancellationToken cancellationToken)
    {
        var costCenter = await _context.CostCenters
            .FirstOrDefaultAsync(cc => cc.Id == request.Id && !cc.IsDeleted, cancellationToken);
        if (costCenter == null)
            throw new NotFoundException(nameof(Domain.Entities.CostCenter), request.Id);

        costCenter.Activate();
        costCenter.UpdatedBy = _currentUserService.UserEmail ?? "system";
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Centro de custo ativado com sucesso.");
    }
}
