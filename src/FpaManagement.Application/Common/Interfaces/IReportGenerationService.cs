using FpaManagement.Domain.Entities;

namespace FpaManagement.Application.Common.Interfaces;

public interface IReportGenerationService
{
    Task<byte[]> GenerateBudgetReportAsync(Budget budget, BudgetVersion version, string format = "PDF");
    Task<byte[]> GenerateCashFlowReportAsync(IEnumerable<CashFlowEntry> entries, DateTime start, DateTime end, string format = "PDF");
    Task<byte[]> GenerateDfcReportAsync(IEnumerable<DfcEntry> entries, int year, string format = "PDF");
    Task<byte[]> GenerateKpiReportAsync(IEnumerable<KpiDefinition> kpis, DateTime period, string format = "PDF");
    // Outros métodos conforme necessário
}
