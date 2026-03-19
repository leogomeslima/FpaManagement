namespace FpaManagement.Application.Common.Interfaces;

public interface IExcelService
{
    byte[] GenerateExcel<T>(IEnumerable<T> data, string sheetName = "Sheet1", Dictionary<string, string>? headers = null);
    Task<List<T>> ImportExcelAsync<T>(byte[] fileData, string sheetName = "Sheet1", CancellationToken cancellationToken = default) where T : new();
    byte[] GenerateTemplate<T>(Dictionary<string, string> fieldMappings);
}
