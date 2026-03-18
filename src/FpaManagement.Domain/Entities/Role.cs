using FpaManagement.Domain.Common;
using FpaManagement.Domain.Enums;

namespace FpaManagement.Domain.Entities;

public class Role : BaseAuditableEntity
{
    public string Name
    {
        get; private set;
    }
    public string? Description
    {
        get; private set;
    }
    public bool IsActive
    {
        get; private set;
    }

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public virtual ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    private Role()
    {
    } // For EF Core

    public Role(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name is required", nameof(name));

        Name = name;
        Description = description;
        IsActive = true;
    }

    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name is required", nameof(name));

        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void Deactivate()
    {
        if (IsActive)
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void AddPermission(Permission permission)
    {
        if (!RolePermissions.Any(rp => rp.Permission == permission && !rp.IsDeleted))
        {
            RolePermissions.Add(new RolePermission(Id, permission));
        }
    }

    public void RemovePermission(Permission permission)
    {
        var rolePermission = RolePermissions.FirstOrDefault(rp => rp.Permission == permission && !rp.IsDeleted);
        if (rolePermission != null)
        {
            rolePermission.Delete();
        }
    }

    public bool HasPermission(Permission permission)
    {
        return RolePermissions.Any(rp => rp.Permission == permission && !rp.IsDeleted);
    }
}
