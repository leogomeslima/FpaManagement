using FpaManagement.Domain.Entities;

namespace FpaManagement.Application.Common.Interfaces;

public interface IPdfService
{
    Task<byte[]> GenerateReportAsync<T>(T data, string templateName, CancellationToken cancellationToken = default);
    byte[] GenerateBudgetReport(Budget budget, BudgetVersion version);
    byte[] GenerateCashFlowReport(IEnumerable<CashFlowEntry> entries, DateTime startDate, DateTime endDate);
    byte[] GenerateDfcReport(IEnumerable<DfcEntry> entries, int year);
}
