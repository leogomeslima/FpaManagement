using AutoMapper;
using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Budget;
using FpaManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.Budget;

[Authorize(Permissions = "ViewBudget")]
public class GetBudgetByIdQuery : IRequest<Result<BudgetDto>>
{
    public Guid Id { get; set; }
}

public class GetBudgetByIdQueryHandler : IRequestHandler<GetBudgetByIdQuery, Result<BudgetDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetBudgetByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<BudgetDto>> Handle(GetBudgetByIdQuery request, CancellationToken cancellationToken)
    {
        var budget = await _context.Budgets
            .Include(b => b.Department)
            .Include(b => b.CostCenter)
            .Include(b => b.Versions.Where(v => !v.IsDeleted))
                .ThenInclude(v => v.Items.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.CostCenter)
            .Include(b => b.Approvals.Where(a => !a.IsDeleted))
                .ThenInclude(a => a.Approver)
            .FirstOrDefaultAsync(b => b.Id == request.Id && !b.IsDeleted, cancellationToken);

        if (budget == null)
        {
            throw new NotFoundException(nameof(Budget), request.Id);
        }

        var result = _mapper.Map<BudgetDto>(budget);
        return Result<BudgetDto>.Success(result);
    }
}
