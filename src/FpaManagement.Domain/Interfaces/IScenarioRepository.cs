using FpaManagement.Domain.Entities;

namespace FpaManagement.Domain.Interfaces;

public interface IScenarioRepository : IGenericRepository<Scenario>
{
    Task<Scenario?> GetScenarioWithAssumptionsAsync(Guid id, CancellationToken cancellationToken = default);
}
