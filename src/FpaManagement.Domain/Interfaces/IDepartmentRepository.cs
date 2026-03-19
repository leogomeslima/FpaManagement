using FpaManagement.Domain.Entities;
namespace FpaManagement.Domain.Interfaces;

public interface IDepartmentRepository : IGenericRepository<Department>
{
    Task<Department?> GetDepartmentWithCostCentersAsync(Guid id, CancellationToken
   cancellationToken = default);
    Task<bool> HasCostCentersAsync(Guid departmentId, CancellationToken cancellationToken = default);
}
