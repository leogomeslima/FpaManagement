using AutoMapper;
using AutoMapper.QueryableExtensions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.CashFlow;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.CashFlow;

[Authorize(Permissions = "ViewCashFlow")]
public class GetCashFlowEntriesQuery : IRequest<PaginatedList<CashFlowEntryDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Domain.Enums.CashFlowType? Type { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? CostCenterId { get; set; }
    public bool? IsReconciled { get; set; }
    public string? SortBy { get; set; } = "Date";
    public bool SortAscending { get; set; } = false;
}

public class GetCashFlowEntriesQueryHandler : IRequestHandler<GetCashFlowEntriesQuery, PaginatedList<CashFlowEntryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCashFlowEntriesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<CashFlowEntryDto>> Handle(GetCashFlowEntriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.CashFlowEntries
            .Include(e => e.Category)
            .Include(e => e.CostCenter)
            .Where(e => !e.IsDeleted);

        if (request.StartDate.HasValue)
            query = query.Where(e => e.Date >= request.StartDate.Value);
        if (request.EndDate.HasValue)
            query = query.Where(e => e.Date <= request.EndDate.Value);
        if (request.Type.HasValue)
            query = query.Where(e => e.Type == request.Type.Value);
        if (request.CategoryId.HasValue)
            query = query.Where(e => e.CategoryId == request.CategoryId.Value);
        if (request.CostCenterId.HasValue)
            query = query.Where(e => e.CostCenterId == request.CostCenterId.Value);
        if (request.IsReconciled.HasValue)
            query = query.Where(e => e.IsReconciled == request.IsReconciled.Value);

        query = request.SortBy?.ToLower() switch
        {
            "date" => request.SortAscending ? query.OrderBy(e => e.Date) : query.OrderByDescending(e => e.Date),
            "amount" => request.SortAscending ? query.OrderBy(e => e.Amount.Amount) : query.OrderByDescending(e => e.Amount.Amount),
            "description" => request.SortAscending ? query.OrderBy(e => e.Description) : query.OrderByDescending(e => e.Description),
            _ => query.OrderByDescending(e => e.Date)
        };

        var projected = query.ProjectTo<CashFlowEntryDto>(_mapper.ConfigurationProvider);
        return await PaginatedList<CashFlowEntryDto>.CreateAsync(projected, request.PageNumber, request.PageSize, cancellationToken);
    }
}
