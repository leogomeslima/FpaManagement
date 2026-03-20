using AutoMapper;
using AutoMapper.QueryableExtensions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Dashboard;
using MediatR;

namespace FpaManagement.Application.Queries.Dashboard;

[Authorize]
public class GetUserDashboardsQuery : IRequest<List<DashboardDefinitionDto>>
{
}

public class GetUserDashboardsQueryHandler : IRequestHandler<GetUserDashboardsQuery, List<DashboardDefinitionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetUserDashboardsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _context = context;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<List<DashboardDefinitionDto>> Handle(GetUserDashboardsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.DashboardDefinitions
            .Include(d => d.DashboardWidgets)
            .Where(d => !d.IsDeleted);

        if (_currentUserService.UserId.HasValue)
        {
            query = query.Where(d => d.UserId == _currentUserService.UserId.Value || d.IsShared);
        }

        return await query
            .OrderBy(d => d.IsDefault ? 0 : 1)
            .ThenBy(d => d.Name)
            .ProjectTo<DashboardDefinitionDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
