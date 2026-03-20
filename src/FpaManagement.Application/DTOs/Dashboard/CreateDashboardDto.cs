namespace FpaManagement.Application.DTOs.Dashboard;

public class CreateDashboardDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
}
