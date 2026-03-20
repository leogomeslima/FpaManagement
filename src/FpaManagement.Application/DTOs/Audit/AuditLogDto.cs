using AutoMapper;
using FpaManagement.Application.Common.Mappings;
using FpaManagement.Application.DTOs.Common;

namespace FpaManagement.Application.DTOs.Audit;

public class AuditLogDto : BaseDto, IMapFrom<Domain.Entities.AuditLog>
{
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? UserEmail { get; set; }
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public string? Changes { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Entities.AuditLog, AuditLogDto>();
    }
}
