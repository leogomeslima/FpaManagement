using FpaManagement.Domain.Enums;
namespace FpaManagement.Application.DTOs.CashFlow;

public class UpdateCashFlowEntryDto
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BRL";
    public CashFlowType Type { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? CostCenterId { get; set; }
    public string? DocumentNumber { get; set; }
}
