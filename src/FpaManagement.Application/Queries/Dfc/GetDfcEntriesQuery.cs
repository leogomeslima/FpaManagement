using AutoMapper;
using AutoMapper.QueryableExtensions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Dfc;
using MediatR;

namespace FpaManagement.Application.Queries.Dfc;

[Authorize(Permissions = "ViewDfc")]
public class GetDfcEntriesQuery : IRequest<PaginatedList<DfcEntryDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int? Year { get; set; }
    public Domain.Enums.DfcActivityType? ActivityType { get; set; }
}

public class GetDfcEntriesQueryHandler : IRequestHandler<GetDfcEntriesQuery, PaginatedList<DfcEntryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDfcEntriesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<DfcEntryDto>> Handle(GetDfcEntriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.DfcEntries.Where(e => !e.IsDeleted);
        if (request.Year.HasValue)
        {
            var start = new DateTime(request.Year.Value, 1, 1);
            var end = new DateTime(request.Year.Value, 12, 31);
            query = query.Where(e => e.Date >= start && e.Date <= end);
        }
        if (request.ActivityType.HasValue)
            query = query.Where(e => e.ActivityType == request.ActivityType.Value);

        query = query.OrderByDescending(e => e.Date);
        var projected = query.ProjectTo<DfcEntryDto>(_mapper.ConfigurationProvider);
        return await PaginatedList<DfcEntryDto>.CreateAsync(projected, request.PageNumber, request.PageSize, cancellationToken);
    }
}
