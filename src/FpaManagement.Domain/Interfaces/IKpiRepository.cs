using FpaManagement.Domain.Entities;

namespace FpaManagement.Domain.Interfaces;

public interface IKpiRepository : IGenericRepository<KpiDefinition>
{
    Task<KpiValue?> GetLatestValueAsync(Guid kpiId, CancellationToken cancellationToken = default);
    Task<IEnumerable<KpiValue>> GetValuesForPeriodAsync(Guid kpiId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
}
