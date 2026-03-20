using AutoMapper;
using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.CostCenter;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.CostCenter;

[Authorize(Permissions = "CreateCostCenter")]
public class CreateCostCenterCommand : IRequest<Result<CostCenterDto>>
{
    public CreateCostCenterDto Data { get; set; } = null!;
}

public class CreateCostCenterCommandHandler : IRequestHandler<CreateCostCenterCommand, Result<CostCenterDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CreateCostCenterCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CostCenterDto>> Handle(CreateCostCenterCommand request, CancellationToken cancellationToken)
    {
        // Verificar se o departamento existe
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == request.Data.DepartmentId && !d.IsDeleted, cancellationToken);

        if (department == null)
        {
            throw new NotFoundException(nameof(Department), request.Data.DepartmentId);
        }

        // Verificar se já existe centro de custo com o mesmo código
        var existingCostCenter = await _context.CostCenters
            .FirstOrDefaultAsync(cc => cc.Code == request.Data.Code && !cc.IsDeleted, cancellationToken);

        if (existingCostCenter != null)
        {
            return Result<CostCenterDto>.Failure($"Já existe um centro de custo com o código '{request.Data.Code}'");
        }

        // Criar centro de custo
        var costCenter = new Domain.Entities.CostCenter(
            request.Data.Code,
            request.Data.Name,
            request.Data.DepartmentId,
            request.Data.Description);

        costCenter.CreatedBy = _currentUserService.UserEmail ?? "system";

        _context.CostCenters.Add(costCenter);
        await _context.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<CostCenterDto>(costCenter);
        return Result<CostCenterDto>.Success(result, "Centro de custo criado com sucesso");
    }
}
