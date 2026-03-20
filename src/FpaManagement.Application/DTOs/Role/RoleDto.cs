using AutoMapper;
using FpaManagement.Application.Common.Mappings;
using FpaManagement.Application.DTOs.Common;

namespace FpaManagement.Application.DTOs.Role;

public class RoleDto : BaseDto, IMapFrom<Domain.Entities.Role>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public List<string> Permissions { get; set; } = new();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Entities.Role, RoleDto>()
            .ForMember(d => d.Permissions, opt => opt.MapFrom(s => s.RolePermissions.Select(rp => rp.Permission.ToString())));
    }
}
