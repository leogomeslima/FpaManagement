namespace FpaManagement.Application.DTOs.CostCenter;

public class UpdateCostCenterDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ManagerId { get; set; }
    public decimal? AnnualBudget { get; set; }
}
