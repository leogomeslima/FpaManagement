using AutoMapper;
using AutoMapper.QueryableExtensions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.DTOs.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.User;

public class GetUserLookupQuery : IRequest<List<LookupDto>>
{
    public bool? IsActive { get; set; } = true;
}

public class GetUserLookupQueryHandler : IRequestHandler<GetUserLookupQuery, List<LookupDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetUserLookupQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<LookupDto>> Handle(GetUserLookupQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users.Where(u => !u.IsDeleted);
        if (request.IsActive.HasValue)
            query = query.Where(u => u.IsActive == request.IsActive.Value);

        return await query
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ProjectTo<LookupDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
