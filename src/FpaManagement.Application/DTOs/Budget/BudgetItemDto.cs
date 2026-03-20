using FpaManagement.Application.Common.Mappings;
using FpaManagement.Application.DTOs.Common;

namespace FpaManagement.Application.DTOs.Budget;

public class BudgetItemDto : BaseDto, IMapFrom<Domain.Entities.BudgetItem>
{
    public string Category { get; set; } = string.Empty;
    public string? SubCategory { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal PlannedAmount { get; set; }
    public decimal? ActualAmount { get; set; }
    public decimal? Variance { get; set; }
    public decimal? VariancePercentage { get; set; }
    public string Currency { get; set; } = "BRL";
    public Guid? CostCenterId { get; set; }
    public string? CostCenterName { get; set; }
    public string? AccountCode { get; set; }

    public void Mapping(AutoMapper.Profile profile)
    {
        profile.CreateMap<Domain.Entities.BudgetItem, BudgetItemDto>()
            .ForMember(d => d.PlannedAmount, opt => opt.MapFrom(s => s.PlannedAmount.Amount))
            .ForMember(d => d.ActualAmount, opt => opt.MapFrom(s => s.ActualAmount != null ? s.ActualAmount.Amount : (decimal?)null))
            .ForMember(d => d.Variance, opt => opt.MapFrom(s => s.Variance != null ? s.Variance.Amount : (decimal?)null))
            .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.PlannedAmount.Currency))
            .ForMember(d => d.CostCenterName, opt => opt.MapFrom(s => s.CostCenter != null ? s.CostCenter.Name : null));
    }
}
