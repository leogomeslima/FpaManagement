using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Domain.Entities;
using FpaManagement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Infrastructure.Repositories;

public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(IApplicationDbContext context) : base(context)
    {
    }

    public async Task<Department?> GetDepartmentWithCostCentersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.Manager)
            .Include(d => d.CostCenters.Where(cc => !cc.IsDeleted))
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted, cancellationToken);
    }

    public async Task<bool> HasCostCentersAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        return await _context.CostCenters
            .AnyAsync(cc => cc.DepartmentId == departmentId && !cc.IsDeleted, cancellationToken);
    }
}
