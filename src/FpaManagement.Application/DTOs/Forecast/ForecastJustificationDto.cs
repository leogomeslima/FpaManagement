using AutoMapper;
using FpaManagement.Application.Common.Mappings;
using FpaManagement.Application.DTOs.Common;
using FpaManagement.Domain.Entities;

namespace FpaManagement.Application.DTOs.Forecast
{
    public class ForecastJustificationDto : BaseDto, IMapFrom<ForecastJustification>
    {
        public string Reason { get; set; } = string.Empty;
        public decimal VariancePercentage { get; set; }
        public string? Comments { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ForecastJustification, ForecastJustificationDto>();
        }
    }
}
