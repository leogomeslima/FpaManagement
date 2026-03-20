using AutoMapper;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Forecast;
using FpaManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Forecast;

[Authorize(Permissions = "CreateForecast")]
public class CreateForecastCommand : IRequest<Result<ForecastDto>>
{
    public CreateForecastDto Data { get; set; } = null!;
}

public class CreateForecastCommandHandler : IRequestHandler<CreateForecastCommand, Result<ForecastDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CreateForecastCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ForecastDto>> Handle(CreateForecastCommand request, CancellationToken cancellationToken)
    {
        var forecast = new Domain.Entities.Forecast(
            request.Data.Name,
            request.Data.FiscalYear,
            request.Data.DepartmentId,
            request.Data.CostCenterId,
            request.Data.Description);

        forecast.CreatedBy = _currentUserService.UserEmail ?? "system";

        var version = forecast.CreateVersion(request.Data.VersionName, request.Data.VersionDescription);
        version.CreatedBy = _currentUserService.UserEmail ?? "system";

        _context.Forecasts.Add(forecast);
        await _context.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<ForecastDto>(forecast);
        return Result<ForecastDto>.Success(result, "Previsão criada com sucesso");
    }
}
