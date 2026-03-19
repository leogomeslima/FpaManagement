using FpaManagement.Application.Common.Mappings;
using FpaManagement.Application.DTOs.Common;

namespace FpaManagement.Application.DTOs.User;

public class UserDto : BaseDto, IMapFrom<Domain.Entities.User>
{
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<string> Roles { get; set; } = new();

    public void Mapping(AutoMapper.Profile profile)
    {
        profile.CreateMap<Domain.Entities.User, UserDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FullName))
            .ForMember(d => d.Roles, opt => opt.MapFrom(s => s.UserRoles
                .Where(ur => !ur.IsDeleted && ur.Role != null && ur.Role.IsActive)
                .Select(ur => ur.Role.Name)));
    }
}
