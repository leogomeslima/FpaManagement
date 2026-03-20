using System.ComponentModel;
using System.Reflection;
using FpaManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace FpaManagement.Infrastructure.Services;

public class ExcelService : IExcelService
{
    private readonly ILogger<ExcelService> _logger;

    public ExcelService(ILogger<ExcelService> logger)
    {
        _logger = logger;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public byte[] GenerateExcel<T>(IEnumerable<T> data, string sheetName = "Sheet1", Dictionary<string, string>? headers = null)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add(sheetName);

        var properties = typeof(T).GetProperties();

        // Set headers
        for (int i = 0; i < properties.Length; i++)
        {
            var header = headers?.GetValueOrDefault(properties[i].Name) ?? properties[i].Name;
            worksheet.Cells[1, i + 1].Value = header;
            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
            worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }

        // Set data
        var row = 2;
        foreach (var item in data)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                var value = properties[i].GetValue(item);
                worksheet.Cells[row, i + 1].Value = value;

                // Format based on type
                if (value is decimal or double or float)
                {
                    worksheet.Cells[row, i + 1].Style.Numberformat.Format = "#,##0.00";
                }
                else if (value is int)
                {
                    worksheet.Cells[row, i + 1].Style.Numberformat.Format = "#,##0";
                }
                else if (value is DateTime)
                {
                    worksheet.Cells[row, i + 1].Style.Numberformat.Format = "dd/MM/yyyy";
                }
            }
            row++;
        }

        // Auto fit columns
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        return package.GetAsByteArray();
    }

    public async Task<List<T>> ImportExcelAsync<T>(byte[] fileData, string sheetName = "Sheet1", CancellationToken cancellationToken = default) where T : new()
    {
        var result = new List<T>();

        using var stream = new MemoryStream(fileData);
        using var package = new ExcelPackage(stream);

        var worksheet = package.Workbook.Worksheets[sheetName] ?? package.Workbook.Worksheets[0];
        if (worksheet == null)
        {
            _logger.LogWarning("No worksheet found in Excel file");
            return result;
        }

        var properties = typeof(T).GetProperties();
        var startRow = 2; // Assume first row is header

        for (int row = startRow; row <= worksheet.Dimension.Rows; row++)
        {
            var item = new T();
            var hasData = false;

            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
            {
                var cellValue = worksheet.Cells[row, col].Value;
                if (cellValue != null)
                {
                    hasData = true;

                    if (col <= properties.Length)
                    {
                        var property = properties[col - 1];
                        var convertedValue = ConvertValue(cellValue, property.PropertyType);
                        property.SetValue(item, convertedValue);
                    }
                }
            }

            if (hasData)
            {
                result.Add(item);
            }
        }

        return await Task.FromResult(result);
    }

    public byte[] GenerateTemplate<T>(Dictionary<string, string> fieldMappings)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Template");

        // Set headers
        int col = 1;
        foreach (var mapping in fieldMappings)
        {
            worksheet.Cells[1, col].Value = mapping.Value;
            worksheet.Cells[1, col].Style.Font.Bold = true;
            worksheet.Cells[1, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[1, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

            // Add comment with field name
            var comment = worksheet.Cells[1, col].AddComment($"Campo: {mapping.Key}", "System");

            col++;
        }

        // Add example row
        worksheet.Cells[2, 1].Value = "Exemplo...";
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        return package.GetAsByteArray();
    }

    private object? ConvertValue(object value, Type targetType)
    {
        try
        {
            if (targetType == typeof(decimal) || targetType == typeof(decimal?))
            {
                return Convert.ToDecimal(value);
            }
            if (targetType == typeof(int) || targetType == typeof(int?))
            {
                return Convert.ToInt32(value);
            }
            if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
            {
                if (value is double doubleValue)
                {
                    return DateTime.FromOADate(doubleValue);
                }
                return Convert.ToDateTime(value);
            }
            if (targetType == typeof(bool) || targetType == typeof(bool?))
            {
                return Convert.ToBoolean(value);
            }
            if (targetType == typeof(Guid) || targetType == typeof(Guid?))
            {
                return Guid.Parse(value.ToString()!);
            }

            return Convert.ChangeType(value, targetType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting value {Value} to {TargetType}", value, targetType);
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }
    }
}
