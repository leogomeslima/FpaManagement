using AutoMapper;
using AutoMapper.QueryableExtensions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.CostCenter;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.CostCenter;

[Authorize(Permissions = "ViewCostCenter")]
public class GetAllCostCentersQuery : IRequest<PaginatedList<CostCenterDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Guid? DepartmentId { get; set; }
    public bool? IsActive { get; set; }
    public string? SortBy { get; set; } = "Name";
    public bool SortAscending { get; set; } = true;
}

public class GetAllCostCentersQueryHandler : IRequestHandler<GetAllCostCentersQuery, PaginatedList<CostCenterDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllCostCentersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<CostCenterDto>> Handle(GetAllCostCentersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.CostCenters
            .Include(cc => cc.Department)
            .Include(cc => cc.Manager)
            .Where(cc => !cc.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(cc => cc.Name.ToLower().Contains(term) || cc.Code.ToLower().Contains(term) ||
                (cc.Description != null && cc.Description.ToLower().Contains(term)));
        }
        if (request.DepartmentId.HasValue)
            query = query.Where(cc => cc.DepartmentId == request.DepartmentId.Value);
        if (request.IsActive.HasValue)
            query = query.Where(cc => cc.IsActive == request.IsActive.Value);

        query = request.SortBy?.ToLower() switch
        {
            "code" => request.SortAscending ? query.OrderBy(cc => cc.Code) : query.OrderByDescending(cc => cc.Code),
            "name" => request.SortAscending ? query.OrderBy(cc => cc.Name) : query.OrderByDescending(cc => cc.Name),
            "department" => request.SortAscending ? query.OrderBy(cc => cc.Department.Name) : query.OrderByDescending(cc => cc.Department.Name),
            "createdat" => request.SortAscending ? query.OrderBy(cc => cc.CreatedAt) : query.OrderByDescending(cc => cc.CreatedAt),
            _ => request.SortAscending ? query.OrderBy(cc => cc.Name) : query.OrderByDescending(cc => cc.Name)
        };

        var projected = query.ProjectTo<CostCenterDto>(_mapper.ConfigurationProvider);
        return await PaginatedList<CostCenterDto>.CreateAsync(projected, request.PageNumber, request.PageSize, cancellationToken);
    }
}
