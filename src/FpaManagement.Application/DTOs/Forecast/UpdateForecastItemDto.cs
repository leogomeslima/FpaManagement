namespace FpaManagement.Application.DTOs.Forecast;

public class UpdateForecastItemDto
{
    public string Category { get; set; } = string.Empty;
    public string? SubCategory { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BRL";
    public Guid? CostCenterId { get; set; }
    public string? AccountCode { get; set; }
}
