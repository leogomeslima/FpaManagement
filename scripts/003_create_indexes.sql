USE FpaManagement
GO

-- Índices para Budgets
CREATE INDEX IX_Budgets_FiscalYear ON [domain].[Budgets] (FiscalYear) INCLUDE (Status)
CREATE INDEX IX_Budgets_Status ON [domain].[Budgets] (Status) INCLUDE (FiscalYear)
CREATE INDEX IX_Budgets_DepartmentId ON [domain].[Budgets] (DepartmentId) WHERE DepartmentId IS NOT NULL
CREATE INDEX IX_Budgets_CostCenterId ON [domain].[Budgets] (CostCenterId) WHERE CostCenterId IS NOT NULL

-- Índices para BudgetItems
CREATE INDEX IX_BudgetItems_BudgetVersionId ON [domain].[BudgetItems] (BudgetVersionId) INCLUDE (Category, PlannedAmount)
CREATE INDEX IX_BudgetItems_Category ON [domain].[BudgetItems] (Category) INCLUDE (BudgetVersionId)

-- Índices para CashFlow
CREATE INDEX IX_CashFlowEntries_Date ON [domain].[CashFlowEntries] (Date) INCLUDE (Type, Amount)
CREATE INDEX IX_CashFlowEntries_CategoryId ON [domain].[CashFlowEntries] (CategoryId) WHERE CategoryId IS NOT NULL
CREATE INDEX IX_CashFlowEntries_Type ON [domain].[CashFlowEntries] (Type) INCLUDE (Date)

-- Índices para AuditLogs
CREATE INDEX IX_AuditLogs_Timestamp ON [audit].[AuditLogs] (Timestamp DESC)
CREATE INDEX IX_AuditLogs_EntityName ON [audit].[AuditLogs] (EntityName) INCLUDE (Action)
CREATE INDEX IX_AuditLogs_UserId ON [audit].[AuditLogs] (UserId) WHERE UserId IS NOT NULL

-- Índices de texto completo para buscas
CREATE FULLTEXT CATALOG ftCatalog AS DEFAULT;
CREATE FULLTEXT INDEX ON [domain].[Budgets](Name, Description) 
    KEY INDEX PK_Budgets WITH STOPLIST = SYSTEM;
