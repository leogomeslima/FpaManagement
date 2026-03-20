using Microsoft.AspNetCore.Identity;

namespace FpaManagement.Infrastructure.Identity;

public class ApplicationUserToken : IdentityUserToken<string>
{
    public virtual ApplicationUser User { get; set; } = null!;
}
