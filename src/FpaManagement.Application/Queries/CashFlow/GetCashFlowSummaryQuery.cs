using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.CashFlow;

[Authorize(Permissions = "ViewCashFlow")]
public class GetCashFlowSummaryQuery : IRequest<CashFlowSummaryDto>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? CostCenterId { get; set; }
}

public class CashFlowSummaryDto
{
    public decimal TotalInflow { get; set; }
    public decimal TotalOutflow { get; set; }
    public decimal NetCashFlow { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal ClosingBalance { get; set; }
    public Dictionary<string, decimal> InflowByCategory { get; set; } = new();
    public Dictionary<string, decimal> OutflowByCategory { get; set; } = new();
}

public class GetCashFlowSummaryQueryHandler : IRequestHandler<GetCashFlowSummaryQuery, CashFlowSummaryDto>
{
    private readonly IApplicationDbContext _context;

    public GetCashFlowSummaryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CashFlowSummaryDto> Handle(GetCashFlowSummaryQuery request, CancellationToken cancellationToken)
    {
        var query = _context.CashFlowEntries
            .Include(e => e.Category)
            .Where(e => !e.IsDeleted && e.Date >= request.StartDate && e.Date <= request.EndDate);

        if (request.CostCenterId.HasValue)
            query = query.Where(e => e.CostCenterId == request.CostCenterId.Value);

        var entries = await query.ToListAsync(cancellationToken);

        var inflow = entries.Where(e => e.Type == Domain.Enums.CashFlowType.Inflow);
        var outflow = entries.Where(e => e.Type == Domain.Enums.CashFlowType.Outflow);

        var totalInflow = inflow.Sum(e => e.Amount.Amount);
        var totalOutflow = outflow.Sum(e => e.Amount.Amount);

        var previousBalance = await _context.CashFlowEntries
            .Where(e => !e.IsDeleted && e.Date < request.StartDate)
            .SumAsync(e => e.Type == Domain.Enums.CashFlowType.Inflow ? e.Amount.Amount : -e.Amount.Amount, cancellationToken);

        var closingBalance = previousBalance + totalInflow - totalOutflow;

        var inflowByCategory = inflow
            .GroupBy(e => e.Category?.Name ?? "Sem categoria")
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount.Amount));

        var outflowByCategory = outflow
            .GroupBy(e => e.Category?.Name ?? "Sem categoria")
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount.Amount));

        return new CashFlowSummaryDto
        {
            TotalInflow = totalInflow,
            TotalOutflow = totalOutflow,
            NetCashFlow = totalInflow - totalOutflow,
            OpeningBalance = previousBalance,
            ClosingBalance = closingBalance,
            InflowByCategory = inflowByCategory,
            OutflowByCategory = outflowByCategory
        };
    }
}
