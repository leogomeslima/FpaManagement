using AutoMapper;
using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.CostCenter;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.CostCenter;

[Authorize(Permissions = "EditCostCenter")]
public class UpdateCostCenterCommand : IRequest<Result<CostCenterDto>>
{
    public Guid Id { get; set; }
    public UpdateCostCenterDto Data { get; set; } = null!;
}

public class UpdateCostCenterCommandHandler : IRequestHandler<UpdateCostCenterCommand, Result<CostCenterDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCostCenterCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CostCenterDto>> Handle(UpdateCostCenterCommand request, CancellationToken cancellationToken)
    {
        var costCenter = await _context.CostCenters
            .FirstOrDefaultAsync(cc => cc.Id == request.Id && !cc.IsDeleted, cancellationToken);
        if (costCenter == null)
            throw new NotFoundException(nameof(Domain.Entities.CostCenter), request.Id);

        costCenter.Update(request.Data.Name, request.Data.Description);
        if (request.Data.ManagerId.HasValue)
            costCenter.SetManager(request.Data.ManagerId.Value);
        if (request.Data.AnnualBudget.HasValue)
            costCenter.SetAnnualBudget(request.Data.AnnualBudget.Value);

        costCenter.UpdatedBy = _currentUserService.UserEmail ?? "system";
        await _context.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<CostCenterDto>(costCenter);
        return Result<CostCenterDto>.Success(dto, "Centro de custo atualizado com sucesso.");
    }
}
