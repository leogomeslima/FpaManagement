using AutoMapper;
using FpaManagement.Application.Common.Mappings;
using FpaManagement.Application.DTOs.Common;
using FpaManagement.Domain.Entities;

namespace FpaManagement.Application.DTOs.CashFlow;

public class CashFlowEntryDto : BaseDto, IMapFrom<CashFlowEntry>
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BRL";
    public string Type { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid? CostCenterId { get; set; }
    public string? CostCenterName { get; set; }
    public string? DocumentNumber { get; set; }
    public bool IsReconciled { get; set; }
    public void Mapping(Profile profile)
    {
        profile.CreateMap<CashFlowEntry, CashFlowEntryDto>()
        .ForMember(d => d.Amount, opt => opt.MapFrom(s => s.Amount.Amount))
        .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.Amount.Currency))
        .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()))
        .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.Name : null))
        .ForMember(d => d.CostCenterName, opt => opt.MapFrom(s => s.CostCenter != null ? s.CostCenter.Name : null));
    }
}
