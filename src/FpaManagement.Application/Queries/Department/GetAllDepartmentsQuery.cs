using AutoMapper;
using AutoMapper.QueryableExtensions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Department;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.Department;

[Authorize(Permissions = "ViewDepartment")]
public class GetAllDepartmentsQuery : IRequest<PaginatedList<DepartmentDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public string? SortBy { get; set; } = "Name";
    public bool SortAscending { get; set; } = true;
}

public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, PaginatedList<DepartmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllDepartmentsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<DepartmentDto>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Departments
            .Include(d => d.Manager)
            .Where(d => !d.IsDeleted);

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            query = query.Where(d =>
                d.Name.ToLower().Contains(search) ||
                d.Code.ToLower().Contains(search) ||
                (d.Description != null && d.Description.ToLower().Contains(search)));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(d => d.IsActive == request.IsActive.Value);
        }

        // Aplicar ordenação
        query = request.SortBy?.ToLower() switch
        {
            "code" => request.SortAscending ? query.OrderBy(d => d.Code) : query.OrderByDescending(d => d.Code),
            "name" => request.SortAscending ? query.OrderBy(d => d.Name) : query.OrderByDescending(d => d.Name),
            "createdat" => request.SortAscending ? query.OrderBy(d => d.CreatedAt) : query.OrderByDescending(d => d.CreatedAt),
            _ => request.SortAscending ? query.OrderBy(d => d.Name) : query.OrderByDescending(d => d.Name)
        };

        // Projetar para DTO e paginar
        var projectedQuery = query.ProjectTo<DepartmentDto>(_mapper.ConfigurationProvider);

        return await PaginatedList<DepartmentDto>.CreateAsync(
            projectedQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
