using Microsoft.AspNetCore.Identity;

namespace FpaManagement.Infrastructure.Identity;

public class ApplicationUserLogin : IdentityUserLogin<string>
{
    public virtual ApplicationUser User { get; set; } = null!;
}
