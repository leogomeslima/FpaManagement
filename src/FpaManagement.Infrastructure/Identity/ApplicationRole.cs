using Microsoft.AspNetCore.Identity;

namespace FpaManagement.Infrastructure.Identity;

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ApplicationRole() : base()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public ApplicationRole(string roleName, string? description = null) : base(roleName)
    {
        Description = description;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }
}
