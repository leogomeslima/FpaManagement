using AutoMapper;
using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Forecast;
using FpaManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.Forecast;

[Authorize(Permissions = "ViewForecast")]
public class GetForecastByIdQuery : IRequest<Result<ForecastDto>>
{
    public Guid Id { get; set; }
}

public class GetForecastByIdQueryHandler : IRequestHandler<GetForecastByIdQuery, Result<ForecastDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetForecastByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<ForecastDto>> Handle(GetForecastByIdQuery request, CancellationToken cancellationToken)
    {
        var forecast = await _context.Forecasts
            .Include(f => f.Department)
            .Include(f => f.CostCenter)
            .Include(f => f.Versions.Where(v => !v.IsDeleted))
                .ThenInclude(v => v.Items.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.CostCenter)
            .Include(f => f.Justifications)
            .FirstOrDefaultAsync(f => f.Id == request.Id && !f.IsDeleted, cancellationToken);

        if (forecast == null)
            throw new NotFoundException(nameof(Forecast), request.Id);

        var result = _mapper.Map<ForecastDto>(forecast);
        return Result<ForecastDto>.Success(result);
    }
}
