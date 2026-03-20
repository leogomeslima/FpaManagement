using AutoMapper;
using AutoMapper.QueryableExtensions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Forecast;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.Forecast;

[Authorize(Permissions = "ViewForecastVersions")]
public class GetForecastVersionsQuery : IRequest<List<ForecastVersionDto>>
{
    public Guid ForecastId { get; set; }
}

public class GetForecastVersionsQueryHandler : IRequestHandler<GetForecastVersionsQuery, List<ForecastVersionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetForecastVersionsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ForecastVersionDto>> Handle(GetForecastVersionsQuery request, CancellationToken cancellationToken)
    {
        var versions = await _context.ForecastVersions
            .Include(v => v.Items)
            .Where(v => v.ForecastId == request.ForecastId && !v.IsDeleted)
            .OrderByDescending(v => v.VersionNumber)
            .ProjectTo<ForecastVersionDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return versions;
    }
}
