using AutoMapper;
using FpaManagement.Application.Common.Mappings;
using FpaManagement.Application.DTOs.Common;

namespace FpaManagement.Application.DTOs.Forecast;

public class ForecastDto : BaseDto, IMapFrom<Domain.Entities.Forecast>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int FiscalYear { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? CostCenterId { get; set; }
    public string? CostCenterName { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "BRL";
    public int VersionsCount { get; set; }
    public ForecastVersionDto? CurrentVersion { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Entities.Forecast, ForecastDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department != null ? s.Department.Name : null))
            .ForMember(d => d.CostCenterName, opt => opt.MapFrom(s => s.CostCenter != null ? s.CostCenter.Name : null))
            .ForMember(d => d.TotalAmount, opt => opt.MapFrom(s => s.TotalAmount != null ? s.TotalAmount.Amount : 0))
            .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.TotalAmount != null ? s.TotalAmount.Currency : "BRL"))
            .ForMember(d => d.VersionsCount, opt => opt.MapFrom(s => s.Versions.Count(v => !v.IsDeleted)))
            .ForMember(d => d.CurrentVersion, opt => opt.MapFrom(s => s.Versions.FirstOrDefault(v => v.IsCurrent && !v.IsDeleted)));
    }
}
