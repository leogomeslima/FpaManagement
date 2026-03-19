using FpaManagement.Domain.Entities;

namespace FpaManagement.Domain.Interfaces;

public interface IForecastRepository : IGenericRepository<Forecast>
{
    Task<Forecast?> GetForecastWithVersionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Forecast>> GetForecastsByYearAsync(int year, CancellationToken cancellationToken = default);
}
