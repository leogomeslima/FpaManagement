using FpaManagement.Application.Common.Mappings;
using FpaManagement.Application.DTOs.Common;
using FpaManagement.Domain.Entities;
using FpaManagement.Domain.Enums;

namespace FpaManagement.Application.DTOs.Budget;

public class BudgetDto : BaseDto, IMapFrom<Domain.Entities.Budget>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int FiscalYear { get; set; }
    public BudgetStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? CostCenterId { get; set; }
    public string? CostCenterName { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "BRL";
    public int VersionsCount { get; set; }
    public BudgetVersionDto? CurrentVersion { get; set; }

    public void Mapping(AutoMapper.Profile profile)
    {
        profile.CreateMap<Domain.Entities.Budget, BudgetDto>()
            .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department != null ? s.Department.Name : null))
            .ForMember(d => d.CostCenterName, opt => opt.MapFrom(s => s.CostCenter != null ? s.CostCenter.Name : null))
            .ForMember(d => d.TotalAmount, opt => opt.MapFrom(s => s.TotalAmount != null ? s.TotalAmount.Amount : 0))
            .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.TotalAmount != null ? s.TotalAmount.Currency : "BRL"))
            .ForMember(d => d.VersionsCount, opt => opt.MapFrom(s => s.Versions.Count(v => !v.IsDeleted)))
            .ForMember(d => d.CurrentVersion, opt => opt.MapFrom(s => s.Versions.FirstOrDefault(v => v.IsCurrent && !v.IsDeleted)));
    }
}
