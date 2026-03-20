using AutoMapper;
using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Forecast;
using FpaManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Commands.Forecast;

[Authorize(Permissions = "EditForecast")]
public class UpdateForecastCommand : IRequest<Result<ForecastDto>>
{
    public Guid Id { get; set; }
    public UpdateForecastDto Data { get; set; } = null!;
}

public class UpdateForecastCommandHandler : IRequestHandler<UpdateForecastCommand, Result<ForecastDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UpdateForecastCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ForecastDto>> Handle(UpdateForecastCommand request, CancellationToken cancellationToken)
    {
        var forecast = await _context.Forecasts
            .FirstOrDefaultAsync(f => f.Id == request.Id && !f.IsDeleted, cancellationToken);

        if (forecast == null)
            throw new NotFoundException(nameof(Forecast), request.Id);

        // Atualizar dados
        // (Nota: a entidade Forecast precisa de um método Update - vamos adicionar)
        forecast.Update(request.Data.Name, request.Data.Description);
        forecast.UpdatedBy = _currentUserService.UserEmail ?? "system";

        await _context.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<ForecastDto>(forecast);
        return Result<ForecastDto>.Success(result, "Previsão atualizada com sucesso");
    }
}
