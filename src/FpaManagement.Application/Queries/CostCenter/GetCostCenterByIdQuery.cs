using AutoMapper;
using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.CostCenter;
using FpaManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.CostCenter;

[Authorize(Permissions = "ViewCostCenter")]
public class GetCostCenterByIdQuery : IRequest<Result<CostCenterDto>>
{
    public Guid Id { get; set; }
}

public class GetCostCenterByIdQueryHandler : IRequestHandler<GetCostCenterByIdQuery, Result<CostCenterDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCostCenterByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<CostCenterDto>> Handle(GetCostCenterByIdQuery request, CancellationToken cancellationToken)
    {
        var costCenter = await _context.CostCenters
            .Include(cc => cc.Department)
            .Include(cc => cc.Manager)
            .FirstOrDefaultAsync(cc => cc.Id == request.Id && !cc.IsDeleted, cancellationToken);
        if (costCenter == null)
            throw new NotFoundException(nameof(Domain.Entities.CostCenter), request.Id);

        var dto = _mapper.Map<CostCenterDto>(costCenter);
        return Result<CostCenterDto>.Success(dto);
    }
}
