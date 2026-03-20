using AutoMapper;
using FpaManagement.Application.Common.Mappings;
using FpaManagement.Application.DTOs.Common;
using FpaManagement.Domain.Entities;

namespace FpaManagement.Application.DTOs.CashFlow;

public class CashFlowProjectionDto : BaseDto, IMapFrom<CashFlowProjection>
{
    public DateTime Date { get; set; }

    public decimal ProjectedInflow { get; set; }

    public decimal ProjectedOutflow { get; set; }

    public decimal ProjectedBalance { get; set; }

    public decimal ActualBalance { get; set; }

    public string Currency { get; set; } = "BRL";

    public Guid? CostCenterId { get; set; }

    public string? CostCenterName { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CashFlowProjection, CashFlowProjectionDto>()
            .ForMember(d => d.ProjectedInflow,
                opt => opt.MapFrom(s => s.ProjectedInflow.Amount))

            .ForMember(d => d.ProjectedOutflow,
                opt => opt.MapFrom(s => s.ProjectedOutflow.Amount))

            .ForMember(d => d.ProjectedBalance,
                opt => opt.MapFrom(s => s.ProjectedBalance.Amount))

            .ForMember(d => d.ActualBalance,
                opt => opt.MapFrom(s => s.ActualBalance.Amount))

            .ForMember(d => d.Currency,
                opt => opt.MapFrom(s => s.ProjectedInflow.Currency))

            .ForMember(d => d.CostCenterName,
                opt => opt.MapFrom(s => s.CostCenter != null
                    ? s.CostCenter.Name
                    : null));
    }
}
