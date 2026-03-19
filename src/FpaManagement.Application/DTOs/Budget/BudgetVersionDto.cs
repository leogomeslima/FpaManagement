using FpaManagement.Application.Common.Mappings;
using FpaManagement.Application.DTOs.Common;
using FpaManagement.Domain.Entities;

namespace FpaManagement.Application.DTOs.Budget;

public class BudgetVersionDto : BaseDto, IMapFrom<Domain.Entities.BudgetVersion>
{
    public string VersionName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int VersionNumber { get; set; }
    public bool IsCurrent { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "BRL";
    public int ItemsCount { get; set; }
    public List<BudgetItemDto> Items { get; set; } = new();

    public void Mapping(AutoMapper.Profile profile)
    {
        profile.CreateMap<Domain.Entities.BudgetVersion, BudgetVersionDto>()
            .ForMember(d => d.TotalAmount, opt => opt.MapFrom(s => s.CalculateTotal().Amount))
            .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.CalculateTotal().Currency))
            .ForMember(d => d.ItemsCount, opt => opt.MapFrom(s => s.Items.Count(i => !i.IsDeleted)));
    }
}
