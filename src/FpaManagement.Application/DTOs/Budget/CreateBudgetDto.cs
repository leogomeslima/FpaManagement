namespace FpaManagement.Application.DTOs.Budget;

public class CreateBudgetDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int FiscalYear { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? CostCenterId { get; set; }
    public string VersionName { get; set; } = "Versão 1";
    public string? VersionDescription { get; set; }
}
