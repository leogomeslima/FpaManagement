using FpaManagement.Domain.Entities;

namespace FpaManagement.Application.DTOs.Common;

public class LookupDto
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<Department, LookupDto>()
                .ForMember(d => d.Code, opt => opt.MapFrom(s => s.Code))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name));

            CreateMap<CostCenter, LookupDto>()
                .ForMember(d => d.Code, opt => opt.MapFrom(s => s.Code))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name));

            CreateMap<User, LookupDto>()
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.FullName));
        }
    }
}
