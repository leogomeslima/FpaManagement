$ErrorActionPreference = "Stop"

function Add-PackageSafe {
    param(
        [string]$Project,
        [string]$Package,
        [string]$Version = ""
    )

    if (-not (Test-Path $Project)) {
        Write-Host "Projeto não encontrado: $Project" -ForegroundColor Red
        return
    }

    if ([string]::IsNullOrWhiteSpace($Version)) {
        Write-Host "Instalando $Package em $Project" -ForegroundColor Cyan
        dotnet add $Project package $Package
    }
    else {
        Write-Host "Instalando $Package $Version em $Project" -ForegroundColor Cyan
        dotnet add $Project package $Package --version $Version
    }
}

Write-Host "Iniciando instalação dos pacotes..." -ForegroundColor Green

# =========================================================
# DOMAIN
# =========================================================
Add-PackageSafe "src/FpaManagement.Domain/FpaManagement.Domain.csproj" "MediatR.Contracts"

# =========================================================
# APPLICATION
# =========================================================
Add-PackageSafe "src/FpaManagement.Application/FpaManagement.Application.csproj" "AutoMapper" "13.0.1"
Add-PackageSafe "src/FpaManagement.Application/FpaManagement.Application.csproj" "FluentValidation" "11.11.0"
Add-PackageSafe "src/FpaManagement.Application/FpaManagement.Application.csproj" "FluentValidation.DependencyInjectionExtensions" "11.11.0"
Add-PackageSafe "src/FpaManagement.Application/FpaManagement.Application.csproj" "MediatR" "12.4.1"
Add-PackageSafe "src/FpaManagement.Application/FpaManagement.Application.csproj" "Microsoft.Extensions.Logging.Abstractions" "10.0.3"

# =========================================================
# INFRASTRUCTURE - EF CORE E BANCO
# =========================================================
Add-PackageSafe "src/FpaManagement.Infrastructure/FpaManagement.Infrastructure.csproj" "Microsoft.EntityFrameworkCore.SqlServer" "10.0.3"
Add-PackageSafe "src/FpaManagement.Infrastructure/FpaManagement.Infrastructure.csproj" "Microsoft.EntityFrameworkCore.Tools" "10.0.3"
Add-PackageSafe "src/FpaManagement.Infrastructure/FpaManagement.Infrastructure.csproj" "Microsoft.EntityFrameworkCore.Design" "10.0.3"

# =========================================================
# INFRASTRUCTURE - IDENTITY E AUTENTICAÇÃO
# =========================================================
Add-PackageSafe "src/FpaManagement.Infrastructure/FpaManagement.Infrastructure.csproj" "Microsoft.AspNetCore.Identity.EntityFrameworkCore" "10.0.3"

# =========================================================
# INFRASTRUCTURE - CACHE
# =========================================================
Add-PackageSafe "src/FpaManagement.Infrastructure/FpaManagement.Infrastructure.csproj" "Microsoft.Extensions.Caching.StackExchangeRedis" "10.0.3"

# =========================================================
# INFRASTRUCTURE - LOGGING
# =========================================================
Add-PackageSafe "src/FpaManagement.Infrastructure/FpaManagement.Infrastructure.csproj" "Serilog" "4.2.0"
Add-PackageSafe "src/FpaManagement.Infrastructure/FpaManagement.Infrastructure.csproj" "Serilog.AspNetCore" "9.0.0"
Add-PackageSafe "src/FpaManagement.Infrastructure/FpaManagement.Infrastructure.csproj" "Serilog.Sinks.Console" "6.0.0"
Add-PackageSafe "src/FpaManagement.Infrastructure/FpaManagement.Infrastructure.csproj" "Serilog.Sinks.File" "6.0.0"
Add-PackageSafe "src/FpaManagement.Infrastructure/FpaManagement.Infrastructure.csproj" "Serilog.Sinks.Seq" "8.0.0"

# =========================================================
# INFRASTRUCTURE - RELATÓRIOS
# =========================================================
Add-PackageSafe "src/FpaManagement.Infrastructure/FpaManagement.Infrastructure.csproj" "EPPlus" "7.5.2"
Add-PackageSafe "src/FpaManagement.Infrastructure/FpaManagement.Infrastructure.csproj" "QuestPDF" "2024.12.2"

# =========================================================
# INFRASTRUCTURE - HEALTH CHECKS
# =========================================================
Add-PackageSafe "src/FpaManagement.Infrastructure/FpaManagement.Infrastructure.csproj" "AspNetCore.HealthChecks.SqlServer" "8.0.2"
Add-PackageSafe "src/FpaManagement.Infrastructure/FpaManagement.Infrastructure.csproj" "AspNetCore.HealthChecks.Redis" "8.0.1"
Add-PackageSafe "src/FpaManagement.Infrastructure/FpaManagement.Infrastructure.csproj" "AspNetCore.HealthChecks.Uris" "8.0.1"

# =========================================================
# API - VERSIONAMENTO
# =========================================================
Add-PackageSafe "src/FpaManagement.API/FpaManagement.API.csproj" "Asp.Versioning.Http" "8.1.0"
Add-PackageSafe "src/FpaManagement.API/FpaManagement.API.csproj" "Asp.Versioning.Mvc.ApiExplorer" "8.1.0"

# =========================================================
# API - JWT
# =========================================================
Add-PackageSafe "src/FpaManagement.API/FpaManagement.API.csproj" "Microsoft.AspNetCore.Authentication.JwtBearer" "10.0.3"

# =========================================================
# API - SWAGGER
# =========================================================
Add-PackageSafe "src/FpaManagement.API/FpaManagement.API.csproj" "Swashbuckle.AspNetCore" "7.2.0"
Add-PackageSafe "src/FpaManagement.API/FpaManagement.API.csproj" "Swashbuckle.AspNetCore.Annotations" "7.2.0"

# =========================================================
# API - RATE LIMIT
# =========================================================
Add-PackageSafe "src/FpaManagement.API/FpaManagement.API.csproj" "AspNetCoreRateLimit" "5.0.0"

# =========================================================
# CLIENT - BLAZOR E AUTENTICAÇÃO
# =========================================================
Add-PackageSafe "src/FpaManagement.Client/FpaManagement.Client.csproj" "Microsoft.AspNetCore.Components.WebAssembly" "10.0.3"
Add-PackageSafe "src/FpaManagement.Client/FpaManagement.Client.csproj" "Microsoft.AspNetCore.Components.WebAssembly.DevServer" "10.0.3"
Add-PackageSafe "src/FpaManagement.Client/FpaManagement.Client.csproj" "Microsoft.AspNetCore.Components.WebAssembly.Authentication" "10.0.3"
Add-PackageSafe "src/FpaManagement.Client/FpaManagement.Client.csproj" "Microsoft.Extensions.Http" "10.0.3"

# =========================================================
# CLIENT - UI
# =========================================================
Add-PackageSafe "src/FpaManagement.Client/FpaManagement.Client.csproj" "MudBlazor" "8.1.0"

# =========================================================
# CLIENT - UTILITÁRIOS E GRÁFICOS
# =========================================================
Add-PackageSafe "src/FpaManagement.Client/FpaManagement.Client.csproj" "Toolbelt.Blazor.HttpClientInterceptor" "10.1.0"
Add-PackageSafe "src/FpaManagement.Client/FpaManagement.Client.csproj" "ChartJs.Blazor.Fork" "2.0.2"
Add-PackageSafe "src/FpaManagement.Client/FpaManagement.Client.csproj" "BlazorSortableJS" "2.1.0"
Add-PackageSafe "src/FpaManagement.Client/FpaManagement.Client.csproj" "Markdig" "0.38.0"

# =========================================================
# TESTES - PACOTES COMUNS
# =========================================================
$testProjects = @(
    "tests/FpaManagement.Domain.Tests/FpaManagement.Domain.Tests.csproj",
    "tests/FpaManagement.Application.Tests/FpaManagement.Application.Tests.csproj",
    "tests/FpaManagement.Infrastructure.Tests/FpaManagement.Infrastructure.Tests.csproj",
    "tests/FpaManagement.Integration.Tests/FpaManagement.Integration.Tests.csproj"
)

foreach ($project in $testProjects) {
    Add-PackageSafe $project "Microsoft.NET.Test.Sdk" "17.12.0"
    Add-PackageSafe $project "xunit" "2.9.2"
    Add-PackageSafe $project "xunit.runner.visualstudio" "3.0.0"
    Add-PackageSafe $project "coverlet.collector" "6.0.2"
    Add-PackageSafe $project "FluentAssertions" "7.0.0"
    Add-PackageSafe $project "Moq" "4.20.72"
}

# =========================================================
# TESTES - APPLICATION
# =========================================================
Add-PackageSafe "tests/FpaManagement.Application.Tests/FpaManagement.Application.Tests.csproj" "FluentValidation.TestHelper" "11.1.1"

# =========================================================
# TESTES - INTEGRAÇÃO
# =========================================================
Add-PackageSafe "tests/FpaManagement.Integration.Tests/FpaManagement.Integration.Tests.csproj" "Microsoft.AspNetCore.Mvc.Testing" "10.0.3"
Add-PackageSafe "tests/FpaManagement.Integration.Tests/FpaManagement.Integration.Tests.csproj" "Testcontainers" "4.1.0"
Add-PackageSafe "tests/FpaManagement.Integration.Tests/FpaManagement.Integration.Tests.csproj" "Testcontainers.MsSql" "4.1.0"
Add-PackageSafe "tests/FpaManagement.Integration.Tests/FpaManagement.Integration.Tests.csproj" "Respawn" "6.2.1"

# =========================================================
# RESTORE FINAL
# =========================================================
Write-Host "Executando restore..." -ForegroundColor Green
dotnet restore

Write-Host ""
Write-Host "Pacotes instalados com sucesso." -ForegroundColor Green