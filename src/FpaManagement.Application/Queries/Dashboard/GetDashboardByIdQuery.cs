using AutoMapper;
using AutoMapper.QueryableExtensions;
using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Dashboard;
using FpaManagement.Domain.Exceptions;
using MediatR;

namespace FpaManagement.Application.Queries.Dashboard;

[Authorize]
public class GetDashboardByIdQuery : IRequest<Result<DashboardDefinitionDto>>
{
    public Guid Id { get; set; }
}

public class GetDashboardByIdQueryHandler : IRequestHandler<GetDashboardByIdQuery, Result<DashboardDefinitionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDashboardByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<DashboardDefinitionDto>> Handle(GetDashboardByIdQuery request, CancellationToken cancellationToken)
    {
        var dashboard = await _context.DashboardDefinitions
            .Include(d => d.DashboardWidgets)
                .ThenInclude(w => w.WidgetDefinition)
            .FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);
        if (dashboard == null)
            throw new NotFoundException(nameof(DashboardDefinition), request.Id);

        var dto = _mapper.Map<DashboardDefinitionDto>(dashboard);
        return Result<DashboardDefinitionDto>.Success(dto);
    }
}
