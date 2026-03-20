using AutoMapper;
using AutoMapper.QueryableExtensions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Budget;
using FpaManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.Budget;

[Authorize(Permissions = "ViewBudget")]
public class GetAllBudgetsQuery : IRequest<PaginatedList<BudgetDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public int? FiscalYear { get; set; }
    public BudgetStatus? Status { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? CostCenterId { get; set; }
    public string? SortBy { get; set; } = "Name";
    public bool SortAscending { get; set; } = true;
}

public class GetAllBudgetsQueryHandler : IRequestHandler<GetAllBudgetsQuery, PaginatedList<BudgetDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllBudgetsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<BudgetDto>> Handle(GetAllBudgetsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Budgets
            .Include(b => b.Department)
            .Include(b => b.CostCenter)
            .Include(b => b.Versions.Where(v => !v.IsDeleted))
            .Where(b => !b.IsDeleted);

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            query = query.Where(b =>
                b.Name.ToLower().Contains(search) ||
                (b.Description != null && b.Description.ToLower().Contains(search)));
        }

        if (request.FiscalYear.HasValue)
        {
            query = query.Where(b => b.FiscalYear == request.FiscalYear.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(b => b.Status == request.Status.Value);
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(b => b.DepartmentId == request.DepartmentId.Value);
        }

        if (request.CostCenterId.HasValue)
        {
            query = query.Where(b => b.CostCenterId == request.CostCenterId.Value);
        }

        // Aplicar ordenação
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.SortAscending ? query.OrderBy(b => b.Name) : query.OrderByDescending(b => b.Name),
            "fiscalyear" => request.SortAscending ? query.OrderBy(b => b.FiscalYear) : query.OrderByDescending(b => b.FiscalYear),
            "status" => request.SortAscending ? query.OrderBy(b => b.Status) : query.OrderByDescending(b => b.Status),
            "createdat" => request.SortAscending ? query.OrderBy(b => b.CreatedAt) : query.OrderByDescending(b => b.CreatedAt),
            "totalamount" => request.SortAscending
                ? query.OrderBy(b => b.TotalAmount != null ? b.TotalAmount.Amount : 0)
                : query.OrderByDescending(b => b.TotalAmount != null ? b.TotalAmount.Amount : 0),
            _ => request.SortAscending ? query.OrderBy(b => b.Name) : query.OrderByDescending(b => b.Name)
        };

        // Projetar para DTO e paginar
        var projectedQuery = query.ProjectTo<BudgetDto>(_mapper.ConfigurationProvider);

        return await PaginatedList<BudgetDto>.CreateAsync(
            projectedQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
