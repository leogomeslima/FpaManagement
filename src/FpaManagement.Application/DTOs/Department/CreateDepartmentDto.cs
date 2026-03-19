namespace FpaManagement.Application.DTOs.Department;

public class CreateDepartmentDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
