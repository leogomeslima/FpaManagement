using FpaManagement.Domain.Entities;

namespace FpaManagement.Domain.Interfaces;

public interface IDemandRepository : IGenericRepository<DemandEntry>
{
    Task<IEnumerable<DemandEntry>> GetByProductAsync(string product, DateTime start, DateTime end, CancellationToken cancellationToken = default);
}
