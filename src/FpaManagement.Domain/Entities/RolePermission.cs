using FpaManagement.Domain.Common;
using FpaManagement.Domain.Enums;

namespace FpaManagement.Domain.Entities;

public class RolePermission : BaseAuditableEntity
{
    public Guid RoleId
    {
        get; private set;
    }
    public Permission Permission
    {
        get; private set;
    }

    // Navigation properties
    public virtual Role Role { get; private set; } = null!;

    private RolePermission()
    {
    } // For EF Core

    public RolePermission(Guid roleId, Permission permission)
    {
        RoleId = roleId;
        Permission = permission;
    }

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}
