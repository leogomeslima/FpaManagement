using FpaManagement.Application.Common.Interfaces;
using FpaManagement.Domain.Interfaces;
using FpaManagement.Infrastructure.Data;
using FpaManagement.Infrastructure.Data.Interceptors;
using FpaManagement.Infrastructure.Identity;
using FpaManagement.Infrastructure.Middlewares;
using FpaManagement.Infrastructure.Repositories;
using FpaManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace FpaManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddScoped<ISaveChangesInterceptor, AuditableEntitySaveChangesInterceptor>();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                });
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        // Identity
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;

            // SignIn settings
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IBudgetRepository, BudgetRepository>();

        // Services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IExcelService, ExcelService>();
        services.AddScoped<IPdfService, PdfService>();

        // Cache (Redis opcional)
        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "FpaManagement";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }
        services.AddScoped<ICacheService, CacheService>();

        // Http Context
        services.AddHttpContextAccessor();

        // Health Checks
        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>("database")
            .AddSqlServer(configuration.GetConnectionString("DefaultConnection")!, name: "sql-server")
            .AddRedis(redisConnectionString ?? "localhost", name: "redis", tags: new[] { "cache" });

        return services;
    }

    public static IServiceCollection AddInfrastructureMiddlewares(this IServiceCollection services)
    {
        services.AddScoped<ExceptionHandlingMiddleware>();
        services.AddScoped<RequestLoggingMiddleware>();

        return services;
    }
}
