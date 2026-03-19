namespace FpaManagement.Application.DTOs.Budget;

public class CreateBudgetItemDto
{
    public Guid BudgetVersionId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? SubCategory { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal PlannedAmount { get; set; }
    public string Currency { get; set; } = "BRL";
    public Guid? CostCenterId { get; set; }
    public string? AccountCode { get; set; }
}
