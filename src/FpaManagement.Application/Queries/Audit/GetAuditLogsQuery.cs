using AutoMapper;
using AutoMapper.QueryableExtensions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Audit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.Audit;

[Authorize(Permissions = "ViewAuditLogs")]
public class GetAuditLogsQuery : IRequest<PaginatedList<AuditLogDto>>
{
    public AuditFilterDto Filter { get; set; } = new();
}

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, PaginatedList<AuditLogDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAuditLogsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<AuditLogDto>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (request.Filter.StartDate.HasValue)
            query = query.Where(a => a.Timestamp >= request.Filter.StartDate.Value);
        if (request.Filter.EndDate.HasValue)
            query = query.Where(a => a.Timestamp <= request.Filter.EndDate.Value);
        if (!string.IsNullOrEmpty(request.Filter.UserEmail))
            query = query.Where(a => a.UserEmail == request.Filter.UserEmail);
        if (!string.IsNullOrEmpty(request.Filter.EntityName))
            query = query.Where(a => a.EntityName == request.Filter.EntityName);
        if (!string.IsNullOrEmpty(request.Filter.Action))
            query = query.Where(a => a.Action == request.Filter.Action);

        query = query.OrderByDescending(a => a.Timestamp);

        var projected = query.ProjectTo<AuditLogDto>(_mapper.ConfigurationProvider);
        return await PaginatedList<AuditLogDto>.CreateAsync(projected, request.Filter.PageNumber, request.Filter.PageSize, cancellationToken);
    }
}
