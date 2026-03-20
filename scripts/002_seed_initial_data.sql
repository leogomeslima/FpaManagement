USE FpaManagement
GO

-- Inserir departamentos iniciais
INSERT INTO [domain].[Departments] (Id, Code, Name, Description, IsActive, CreatedAt, CreatedBy)
VALUES 
    (NEWID(), 'FIN', 'Financeiro', 'Departamento Financeiro', 1, GETUTCDATE(), 'system'),
    (NEWID(), 'COM', 'Comercial', 'Departamento Comercial', 1, GETUTCDATE(), 'system'),
    (NEWID(), 'RH', 'Recursos Humanos', 'Departamento de RH', 1, GETUTCDATE(), 'system'),
    (NEWID(), 'IT', 'Tecnologia da Informação', 'Departamento de TI', 1, GETUTCDATE(), 'system'),
    (NEWID(), 'OPS', 'Operações', 'Departamento de Operações', 1, GETUTCDATE(), 'system')
GO

-- Inserir centros de custo
INSERT INTO [domain].[CostCenters] (Id, Code, Name, Description, DepartmentId, IsActive, CreatedAt, CreatedBy)
SELECT 
    NEWID(),
    'FIN-ADM',
    'Administração Financeira',
    'Centro de custo da administração financeira',
    Id,
    1,
    GETUTCDATE(),
    'system'
FROM [domain].[Departments] WHERE Code = 'FIN'
GO

INSERT INTO [domain].[CostCenters] (Id, Code, Name, Description, DepartmentId, IsActive, CreatedAt, CreatedBy)
SELECT 
    NEWID(),
    'COM-VENDAS',
    'Vendas',
    'Centro de custo de vendas',
    Id,
    1,
    GETUTCDATE(),
    'system'
FROM [domain].[Departments] WHERE Code = 'COM'
GO

-- Inserir categorias financeiras
INSERT INTO [domain].[FinancialCategories] (Id, Name, Type, IsActive, CreatedAt, CreatedBy)
VALUES 
    (NEWID(), 'Receita de Vendas', 1, 1, GETUTCDATE(), 'system'),
    (NEWID(), 'Receita de Serviços', 1, 1, GETUTCDATE(), 'system'),
    (NEWID(), 'Custos com Pessoal', 2, 1, GETUTCDATE(), 'system'),
    (NEWID(), 'Despesas Operacionais', 2, 1, GETUTCDATE(), 'system'),
    (NEWID(), 'Investimentos', 3, 1, GETUTCDATE(), 'system')
GO

-- Inserir perfis (roles)
INSERT INTO [identity].[Roles] (Id, Name, NormalizedName, Description, IsActive, CreatedAt)
VALUES 
    (NEWID(), 'Admin', 'ADMIN', 'Administrador do sistema', 1, GETUTCDATE()),
    (NEWID(), 'Controller', 'CONTROLLER', 'Controller financeiro', 1, GETUTCDATE()),
    (NEWID(), 'Financeiro', 'FINANCEIRO', 'Usuário do financeiro', 1, GETUTCDATE()),
    (NEWID(), 'Gestor', 'GESTOR', 'Gestor de área', 1, GETUTCDATE())
GO
