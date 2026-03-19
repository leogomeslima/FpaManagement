using FpaManagement.Domain.Entities;
using FpaManagement.Domain.Enums;

namespace FpaManagement.Domain.Interfaces;

public interface IDfcRepository : IGenericRepository<DfcEntry>
{
    Task<IEnumerable<DfcEntry>> GetByActivityTypeAsync(DfcActivityType type, int year, CancellationToken cancellationToken = default);
}
