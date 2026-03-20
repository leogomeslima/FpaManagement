using AutoMapper;
using AutoMapper.QueryableExtensions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Budget;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.Budget;

[Authorize(Permissions = "ViewBudgetVersions")]
public class GetBudgetVersionsQuery : IRequest<List<BudgetVersionDto>>
{
    public Guid BudgetId { get; set; }
}

public class GetBudgetVersionsQueryHandler : IRequestHandler<GetBudgetVersionsQuery, List<BudgetVersionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetBudgetVersionsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<BudgetVersionDto>> Handle(GetBudgetVersionsQuery request, CancellationToken cancellationToken)
    {
        var versions = await _context.BudgetVersions
            .Include(v => v.Items)
            .Where(v => v.BudgetId == request.BudgetId && !v.IsDeleted)
            .OrderByDescending(v => v.VersionNumber)
            .ProjectTo<BudgetVersionDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return versions;
    }
}
