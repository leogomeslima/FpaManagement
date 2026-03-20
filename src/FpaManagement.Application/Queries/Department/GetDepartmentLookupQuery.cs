using AutoMapper;
using AutoMapper.QueryableExtensions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.DTOs.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.Department;

public class GetDepartmentLookupQuery : IRequest<List<LookupDto>>
{
    public bool? IsActive { get; set; } = true;
}

public class GetDepartmentLookupQueryHandler : IRequestHandler<GetDepartmentLookupQuery, List<LookupDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDepartmentLookupQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<LookupDto>> Handle(GetDepartmentLookupQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Departments
            .Where(d => !d.IsDeleted);

        if (request.IsActive.HasValue)
        {
            query = query.Where(d => d.IsActive == request.IsActive.Value);
        }

        return await query
            .OrderBy(d => d.Name)
            .ProjectTo<LookupDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
