using AutoMapper;
using FpaManagement.Application.Common.Mappings;
using FpaManagement.Application.DTOs.Common;
using FpaManagement.Domain.Entities;

namespace FpaManagement.Application.DTOs.Dashboard;

public class DashboardDefinitionDto : BaseDto, IMapFrom<DashboardDefinition>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public bool IsDefault { get; set; }
    public bool IsShared { get; set; }
    public string Layout { get; set; } = "{}";
    public List<DashboardWidgetDto> Widgets { get; set; } = new();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<DashboardDefinition, DashboardDefinitionDto>()
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User != null ? s.User.FullName : null))
            .ForMember(d => d.Widgets, opt => opt.MapFrom(s => s.DashboardWidgets));
    }
}
