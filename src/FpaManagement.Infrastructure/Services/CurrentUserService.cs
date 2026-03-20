using System.Security.Claims;
using FpaManagement.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FpaManagement.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue("UserId");
            return userIdClaim != null ? Guid.Parse(userIdClaim) : null;
        }
    }

    public string? UserEmail => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public IEnumerable<string> Roles => _httpContextAccessor.HttpContext?.User?
        .FindAll(ClaimTypes.Role)
        .Select(c => c.Value) ?? Enumerable.Empty<string>();

    public bool HasPermission(string permission)
    {
        return _httpContextAccessor.HttpContext?.User?
            .HasClaim(c => c.Type == "Permission" && c.Value == permission) ?? false;
    }

    public bool IsInRole(string role)
    {
        return _httpContextAccessor.HttpContext?.User?
            .IsInRole(role) ?? false;
    }
}
