using FpaManagement.Application.Common.Mappings;
using FpaManagement.Application.DTOs.Common;
using FpaManagement.Domain.Entities;

namespace FpaManagement.Application.DTOs.Department;

public class DepartmentDto : BaseDto, IMapFrom<Domain.Entities.Department>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public int CostCentersCount { get; set; }

    public void Mapping(AutoMapper.Profile profile)
    {
        profile.CreateMap<Domain.Entities.Department, DepartmentDto>()
            .ForMember(d => d.ManagerName, opt => opt.MapFrom(s => s.Manager != null ? s.Manager.FullName : null))
            .ForMember(d => d.CostCentersCount, opt => opt.MapFrom(s => s.CostCenters.Count(c => !c.IsDeleted)));
    }
}
