using AutoMapper;
using FpaManagement.Application.Common.Mappings;
using FpaManagement.Application.DTOs.Common;
using FpaManagement.Domain.Entities;
namespace FpaManagement.Application.DTOs.CashFlow;

public class CashFlowCategoryDto : BaseDto, IMapFrom<CashFlowCategory>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public void Mapping(Profile profile)
    {
        profile.CreateMap<CashFlowCategory, CashFlowCategoryDto>()
        .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()));
    }
}
