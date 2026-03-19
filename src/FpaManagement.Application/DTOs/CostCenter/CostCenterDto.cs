using FpaManagement.Application.Common.Mappings;
using FpaManagement.Application.DTOs.Common;
using FpaManagement.Domain.Entities;

namespace FpaManagement.Application.DTOs.CostCenter;

public class CostCenterDto : BaseDto, IMapFrom<Domain.Entities.CostCenter>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public decimal? AnnualBudget { get; set; }

    public void Mapping(AutoMapper.Profile profile)
    {
        profile.CreateMap<Domain.Entities.CostCenter, CostCenterDto>()
            .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department.Name))
            .ForMember(d => d.ManagerName, opt => opt.MapFrom(s => s.Manager != null ? s.Manager.FullName : null));
    }
}
