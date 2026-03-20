using AutoMapper;
using FpaManagement.Application.Common.Exceptions;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Dfc;
using FpaManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.Dfc;

[Authorize(Permissions = "ViewDfc")]
public class GetDfcEntryByIdQuery : IRequest<Result<DfcEntryDto>>
{
    public Guid Id { get; set; }
}

public class GetDfcEntryByIdQueryHandler : IRequestHandler<GetDfcEntryByIdQuery, Result<DfcEntryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDfcEntryByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<DfcEntryDto>> Handle(GetDfcEntryByIdQuery request, CancellationToken cancellationToken)
    {
        var entry = await _context.DfcEntries
            .FirstOrDefaultAsync(e => e.Id == request.Id && !e.IsDeleted, cancellationToken);
        if (entry == null)
            throw new NotFoundException(nameof(DfcEntry), request.Id);

        var dto = _mapper.Map<DfcEntryDto>(entry);
        return Result<DfcEntryDto>.Success(dto);
    }
}
