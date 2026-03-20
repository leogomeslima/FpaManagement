using AutoMapper;
using AutoMapper.QueryableExtensions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.DTOs.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.CostCenter;

public class GetCostCenterLookupQuery : IRequest<List<LookupDto>>
{
    public bool? IsActive { get; set; } = true;
    public Guid? DepartmentId { get; set; }
}

public class GetCostCenterLookupQueryHandler : IRequestHandler<GetCostCenterLookupQuery, List<LookupDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCostCenterLookupQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<LookupDto>> Handle(GetCostCenterLookupQuery request, CancellationToken cancellationToken)
    {
        var query = _context.CostCenters.Where(cc => !cc.IsDeleted);
        if (request.IsActive.HasValue)
            query = query.Where(cc => cc.IsActive == request.IsActive.Value);
        if (request.DepartmentId.HasValue)
            query = query.Where(cc => cc.DepartmentId == request.DepartmentId.Value);

        return await query
            .OrderBy(cc => cc.Name)
            .ProjectTo<LookupDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
