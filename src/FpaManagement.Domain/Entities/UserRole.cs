using FpaManagement.Domain.Common;

namespace FpaManagement.Domain.Entities;

public class UserRole : BaseAuditableEntity
{
    public Guid UserId
    {
        get; private set;
    }
    public Guid RoleId
    {
        get; private set;
    }

    // Navigation properties
    public virtual User User { get; private set; } = null!;
    public virtual Role Role { get; private set; } = null!;

    private UserRole()
    {
    } // For EF Core

    public UserRole(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}
