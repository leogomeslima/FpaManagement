using AutoMapper;
using AutoMapper.QueryableExtensions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.CostCenter;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.CostCenter;

[Authorize(Permissions = "ViewCostCenter")]
public class GetCostCentersByDepartmentQuery : IRequest<List<CostCenterDto>>
{
    public Guid DepartmentId { get; set; }
    public bool? IsActive { get; set; } = true;
}

public class GetCostCentersByDepartmentQueryHandler : IRequestHandler<GetCostCentersByDepartmentQuery, List<CostCenterDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCostCentersByDepartmentQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<CostCenterDto>> Handle(GetCostCentersByDepartmentQuery request, CancellationToken cancellationToken)
    {
        var query = _context.CostCenters
            .Include(cc => cc.Department)
            .Include(cc => cc.Manager)
            .Where(cc => cc.DepartmentId == request.DepartmentId && !cc.IsDeleted);
        if (request.IsActive.HasValue)
            query = query.Where(cc => cc.IsActive == request.IsActive.Value);

        return await query
            .OrderBy(cc => cc.Name)
            .ProjectTo<CostCenterDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
