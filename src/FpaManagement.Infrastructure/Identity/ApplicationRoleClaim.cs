using Microsoft.AspNetCore.Identity;

namespace FpaManagement.Infrastructure.Identity;

public class ApplicationRoleClaim : IdentityRoleClaim<string>
{
    public virtual ApplicationRole Role { get; set; } = null!;
}
