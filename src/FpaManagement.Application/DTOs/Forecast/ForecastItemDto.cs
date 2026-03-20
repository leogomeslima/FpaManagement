using AutoMapper;
using FpaManagement.Application.Common.Mappings;
using FpaManagement.Application.DTOs.Common;

namespace FpaManagement.Application.DTOs.Forecast;

public class ForecastItemDto : BaseDto, IMapFrom<Domain.Entities.ForecastItem>
{
    public string Category { get; set; } = string.Empty;
    public string? SubCategory { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BRL";
    public Guid? CostCenterId { get; set; }
    public string? CostCenterName { get; set; }
    public string? AccountCode { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Entities.ForecastItem, ForecastItemDto>()
            .ForMember(d => d.Amount, opt => opt.MapFrom(s => s.Amount.Amount))
            .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.Amount.Currency))
            .ForMember(d => d.CostCenterName, opt => opt.MapFrom(s => s.CostCenter != null ? s.CostCenter.Name : null));
    }
}
