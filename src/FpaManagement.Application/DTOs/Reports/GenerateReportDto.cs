namespace FpaManagement.Application.DTOs.Reports;

public class GenerateReportDto
{
    public string ReportType { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public string Format { get; set; } = "PDF"; // PDF, Excel
}
