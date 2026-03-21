using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Variance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.Variance;

[Authorize(Permissions = "ViewVariance")]
public class GetBudgetVsActualVarianceQuery : IRequest<VarianceReportDto>
{
    public Guid BudgetId { get; set; }
    public DateTime? AsOfDate { get; set; }
}

public class GetBudgetVsActualVarianceQueryHandler : IRequestHandler<GetBudgetVsActualVarianceQuery, VarianceReportDto>
{
    private readonly IApplicationDbContext _context;

    public GetBudgetVsActualVarianceQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VarianceReportDto> Handle(GetBudgetVsActualVarianceQuery request, CancellationToken cancellationToken)
    {
        var budget = await _context.Budgets
            .Include(b => b.Versions.Where(v => v.IsCurrent))
                .ThenInclude(v => v.Items)
            .FirstOrDefaultAsync(b => b.Id == request.BudgetId && !b.IsDeleted, cancellationToken);

        if (budget == null)
            throw new NotFoundException(nameof(Domain.Entities.Budget), request.BudgetId);

        var currentVersion = budget.Versions.FirstOrDefault(v => v.IsCurrent);
        if (currentVersion == null)
            return new VarianceReportDto { Title = "Nenhuma versão atual encontrada" };

        var asOfDate = request.AsOfDate ?? DateTime.UtcNow;
        var startDate = new DateTime(budget.FiscalYear, 1, 1);
        var endDate = asOfDate;

        // 1. Obter valores reais filtrando nulos e garantindo que a chave seja Guid (não Guid?)
        var actualRevenues = await _context.Revenues
            .Where(r => r.Date >= startDate && r.Date <= endDate && !r.IsDeleted && r.CategoryId != null)
            .GroupBy(r => r.CategoryId)
            .Select(g => new { CategoryId = g.Key!.Value, Total = g.Sum(r => r.Amount.Amount) })
            .ToDictionaryAsync(x => x.CategoryId, x => x.Total, cancellationToken);

        var actualExpenses = await _context.Expenses
            .Where(e => e.Date >= startDate && e.Date <= endDate && !e.IsDeleted && e.CategoryId != null)
            .GroupBy(e => e.CategoryId)
            .Select(g => new { CategoryId = g.Key!.Value, Total = g.Sum(e => e.Amount.Amount) })
            .ToDictionaryAsync(x => x.CategoryId, x => x.Total, cancellationToken);

        var items = new List<VarianceAnalysisDto>();

        foreach (var item in currentVersion.Items)
        {
            var planned = item.PlannedAmount.Amount;
            var actual = 0m;

            if (item.Category == "Revenue" && item.CostCenterId.HasValue)
            {
                actual = actualRevenues.GetValueOrDefault(item.CostCenterId.Value, 0m);
            }
            else if (item.Category == "Expense" && item.CostCenterId.HasValue)
            {
                actual = actualExpenses.GetValueOrDefault(item.CostCenterId.Value, 0m);
            }

            var variance = actual - planned;
            var variancePct = planned != 0 ? (variance / planned) * 100 : 0;

            // Lógica de Status: Receita maior que planejado é favorável, Despesa maior é desfavorável
            bool isFavorable = item.Category == "Revenue" ? variance >= 0 : variance <= 0;

            items.Add(new VarianceAnalysisDto
            {
                Category = item.Category,
                Planned = planned,
                Actual = actual,
                Variance = variance,
                VariancePercentage = variancePct,
                Status = isFavorable ? "Favorable" : "Unfavorable"
            });
        }

        // Totais consolidados
        var totalPlanned = items.Sum(i => i.Planned);
        var totalActual = items.Sum(i => i.Actual);
        var totalVariance = totalActual - totalPlanned;
        var totalVariancePct = totalPlanned != 0 ? (totalVariance / totalPlanned) * 100 : 0;

        return new VarianceReportDto
        {
            Title = $"Variação Orçado vs Realizado - {budget.Name} ({budget.FiscalYear})",
            Period = asOfDate,
            Items = items,
            TotalPlanned = totalPlanned,
            TotalActual = totalActual,
            TotalVariance = totalVariance,
            TotalVariancePercentage = totalVariancePct
        };
    }
}
