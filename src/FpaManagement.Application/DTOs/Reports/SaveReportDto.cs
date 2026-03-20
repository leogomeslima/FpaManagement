namespace FpaManagement.Application.DTOs.Reports;

public class SaveReportDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public string Parameters { get; set; } = string.Empty;
    public bool IsScheduled { get; set; }
    public string? ScheduleCron { get; set; }
}
