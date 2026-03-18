namespace FpaManagement.Domain.Enums
{
    public enum Permission
    {
        // Dashboard
        ViewDashboard = 1,
        ExportDashboard = 2,

        // Budget
        ViewBudget = 10,
        CreateBudget = 11,
        EditBudget = 12,
        DeleteBudget = 13,
        ApproveBudget = 14,
        ViewBudgetVersions = 15,

        // Forecast
        ViewForecast = 20,
        CreateForecast = 21,
        EditForecast = 22,
        DeleteForecast = 23,
        ApproveForecast = 24,

        // CashFlow
        ViewCashFlow = 30,
        CreateCashFlow = 31,
        EditCashFlow = 32,
        DeleteCashFlow = 33,

        // DFC
        ViewDfc = 40,
        GenerateDfc = 41,

        // Revenue/Expense
        ViewTransactions = 50,
        CreateTransaction = 51,
        EditTransaction = 52,
        DeleteTransaction = 53,

        // KPIs
        ViewKpis = 60,
        ManageKpis = 61,

        // Reports
        ViewReports = 70,
        GenerateReports = 71,
        ExportReports = 72,

        // Administration
        ManageUsers = 80,
        ManageRoles = 81,
        ViewAuditLogs = 82,
        ManageSettings = 83
    }
}
