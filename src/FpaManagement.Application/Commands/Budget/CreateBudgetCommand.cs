using AutoMapper;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Budget;
using FpaManagement.Domain.Entities;
using FpaManagement.Domain.ValueObjects;
using MediatR;

namespace FpaManagement.Application.Commands.Budget;

[Authorize(Permissions = "CreateBudget")]
public class CreateBudgetCommand : IRequest<Result<BudgetDto>>
{
    public CreateBudgetDto Data { get; set; } = null!;
}

public class CreateBudgetCommandHandler : IRequestHandler<CreateBudgetCommand, Result<BudgetDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CreateBudgetCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Result<BudgetDto>> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        // Criar orçamento
        var budget = new Domain.Entities.Budget(
            request.Data.Name,
            request.Data.FiscalYear,
            request.Data.DepartmentId,
            request.Data.CostCenterId,
            request.Data.Description);

        budget.CreatedBy = _currentUserService.UserEmail ?? "system";

        // Criar versão inicial
        var version = budget.CreateVersion(request.Data.VersionName, request.Data.VersionDescription);
        version.CreatedBy = _currentUserService.UserEmail ?? "system";

        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<BudgetDto>(budget);
        return Result<BudgetDto>.Success(result, "Orçamento criado com sucesso");
    }
}
