using AutoMapper;
using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Dfc;
using FpaManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FpaManagement.Application.Queries.Dfc;

[Authorize(Permissions = "ViewDfc")]
public class GetDfcReportQuery : IRequest<DfcReportDto>
{
    public int Year { get; set; }
}

public class DfcReportDto
{
    public int Year { get; set; }
    public decimal OperatingActivities { get; set; }
    public decimal InvestingActivities { get; set; }
    public decimal FinancingActivities { get; set; }
    public decimal NetCashFlow { get; set; }
    public List<DfcEntryDto> Entries { get; set; } = new();
}

public class GetDfcReportQueryHandler : IRequestHandler<GetDfcReportQuery, DfcReportDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDfcReportQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DfcReportDto> Handle(GetDfcReportQuery request, CancellationToken cancellationToken)
    {
        var start = new DateTime(request.Year, 1, 1);
        var end = new DateTime(request.Year, 12, 31);

        var entries = await _context.DfcEntries
            .Where(e => e.Date >= start && e.Date <= end && !e.IsDeleted)
            .ToListAsync(cancellationToken);

        var operating = entries.Where(e => e.ActivityType == DfcActivityType.Operating).Sum(e => e.Amount.Amount);
        var investing = entries.Where(e => e.ActivityType == DfcActivityType.Investing).Sum(e => e.Amount.Amount);
        var financing = entries.Where(e => e.ActivityType == DfcActivityType.Financing).Sum(e => e.Amount.Amount);

        return new DfcReportDto
        {
            Year = request.Year,
            OperatingActivities = operating,
            InvestingActivities = investing,
            FinancingActivities = financing,
            NetCashFlow = operating + investing + financing,
            Entries = _mapper.Map<List<DfcEntryDto>>(entries)
        };
    }
}
