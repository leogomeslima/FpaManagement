using FpaManagement.Domain.Enums;

namespace FpaManagement.Application.DTOs.Dfc;

public class CreateDfcEntryDto
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BRL";
    public DfcActivityType ActivityType { get; set; }
    public Guid? CashFlowEntryId { get; set; }
}
