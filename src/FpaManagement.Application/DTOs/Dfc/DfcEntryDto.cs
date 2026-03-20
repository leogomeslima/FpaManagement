using AutoMapper;
using FpaManagement.Application.Common.Mappings;
using FpaManagement.Application.DTOs.Common;
using FpaManagement.Domain.Entities;

namespace FpaManagement.Application.DTOs.Dfc;

public class DfcEntryDto : BaseDto, IMapFrom<DfcEntry>
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BRL";
    public string ActivityType { get; set; } = string.Empty;
    public Guid? CashFlowEntryId { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<DfcEntry, DfcEntryDto>()
            .ForMember(d => d.Amount, opt => opt.MapFrom(s => s.Amount.Amount))
            .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.Amount.Currency))
            .ForMember(d => d.ActivityType, opt => opt.MapFrom(s => s.ActivityType.ToString()));
    }
}
