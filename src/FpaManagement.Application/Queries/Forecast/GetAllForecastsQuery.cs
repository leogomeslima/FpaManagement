using AutoMapper;
using AutoMapper.QueryableExtensions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Forecast;
using FpaManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.Forecast;

[Authorize(Permissions = "ViewForecast")]
public class GetAllForecastsQuery : IRequest<PaginatedList<ForecastDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public int? FiscalYear { get; set; }
    public ForecastStatus? Status { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? CostCenterId { get; set; }
    public string? SortBy { get; set; } = "Name";
    public bool SortAscending { get; set; } = true;
}

public class GetAllForecastsQueryHandler : IRequestHandler<GetAllForecastsQuery, PaginatedList<ForecastDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllForecastsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ForecastDto>> Handle(GetAllForecastsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Forecasts
            .Include(f => f.Department)
            .Include(f => f.CostCenter)
            .Include(f => f.Versions.Where(v => !v.IsDeleted))
            .Where(f => !f.IsDeleted);

        // Filtros
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            query = query.Where(f => f.Name.ToLower().Contains(search) || (f.Description != null && f.Description.ToLower().Contains(search)));
        }

        if (request.FiscalYear.HasValue)
            query = query.Where(f => f.FiscalYear == request.FiscalYear.Value);

        if (request.Status.HasValue)
            query = query.Where(f => f.Status == request.Status.Value);

        if (request.DepartmentId.HasValue)
            query = query.Where(f => f.DepartmentId == request.DepartmentId.Value);

        if (request.CostCenterId.HasValue)
            query = query.Where(f => f.CostCenterId == request.CostCenterId.Value);

        // Ordenação
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.SortAscending ? query.OrderBy(f => f.Name) : query.OrderByDescending(f => f.Name),
            "fiscalyear" => request.SortAscending ? query.OrderBy(f => f.FiscalYear) : query.OrderByDescending(f => f.FiscalYear),
            "status" => request.SortAscending ? query.OrderBy(f => f.Status) : query.OrderByDescending(f => f.Status),
            "totalamount" => request.SortAscending
                ? query.OrderBy(f => f.TotalAmount != null ? f.TotalAmount.Amount : 0)
                : query.OrderByDescending(f => f.TotalAmount != null ? f.TotalAmount.Amount : 0),
            _ => request.SortAscending ? query.OrderBy(f => f.Name) : query.OrderByDescending(f => f.Name)
        };

        var projected = query.ProjectTo<ForecastDto>(_mapper.ConfigurationProvider);
        return await PaginatedList<ForecastDto>.CreateAsync(projected, request.PageNumber, request.PageSize, cancellationToken);
    }
}
