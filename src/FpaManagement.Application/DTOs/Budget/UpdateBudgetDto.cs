using FpaManagement.Domain.Enums;

namespace FpaManagement.Application.DTOs.Budget;

public class UpdateBudgetDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public BudgetStatus Status { get; set; }
}
