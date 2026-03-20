using AutoMapper;
using FpaManagement.Application.Common.Mappings;
using FpaManagement.Application.DTOs.Common;

namespace FpaManagement.Application.DTOs.Reports;

public class ReportDto : BaseDto, IMapFrom<Domain.Entities.Report>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public string Parameters { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public bool IsScheduled { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Entities.Report, ReportDto>();
    }
}
