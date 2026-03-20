using AutoMapper;
using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.CashFlow;
using FpaManagement.Domain.Entities;
using FpaManagement.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.CashFlow;

[Authorize(Permissions = "ViewCashFlow")]
public class GetCashFlowEntryByIdQuery : IRequest<Result<CashFlowEntryDto>>
{
    public Guid Id { get; set; }
}

public class GetCashFlowEntryByIdQueryHandler : IRequestHandler<GetCashFlowEntryByIdQuery, Result<CashFlowEntryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCashFlowEntryByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<CashFlowEntryDto>> Handle(GetCashFlowEntryByIdQuery request, CancellationToken cancellationToken)
    {
        var entry = await _context.CashFlowEntries
            .Include(e => e.Category)
            .Include(e => e.CostCenter)
            .FirstOrDefaultAsync(e => e.Id == request.Id && !e.IsDeleted, cancellationToken);
        if (entry == null)
            throw new NotFoundException(nameof(CashFlowEntry), request.Id);

        var dto = _mapper.Map<CashFlowEntryDto>(entry);
        return Result<CashFlowEntryDto>.Success(dto);
    }
}
