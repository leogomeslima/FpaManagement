using FpaManagement.Domain.Common;
using FpaManagement.Domain.Enums;

namespace FpaManagement.Domain.Entities;

public class User : BaseAuditableEntity
{
    public string Email
    {
        get; private set;
    }
    public string UserName
    {
        get; private set;
    }
    public string FirstName
    {
        get; private set;
    }
    public string LastName
    {
        get; private set;
    }
    public string FullName => $"{FirstName} {LastName}";
    public string? PhoneNumber
    {
        get; private set;
    }
    public bool IsActive
    {
        get; private set;
    }
    public DateTime? LastLoginAt
    {
        get; private set;
    }
    public string? RefreshToken
    {
        get; private set;
    }
    public DateTime? RefreshTokenExpiryTime
    {
        get; private set;
    }
    public string? PasswordHash
    {
        get; private set;
    } // Para integração com Identity

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public virtual ICollection<AuditLog> AuditLogs { get; private set; } = new List<AuditLog>();
    public virtual ICollection<BudgetApproval> BudgetApprovals { get; private set; } = new List<BudgetApproval>();

    private User()
    {
    } // For EF Core

    public User(string email, string userName, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Username is required", nameof(userName));

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required", nameof(lastName));

        Id = Guid.NewGuid();
        Email = email.ToLowerInvariant();
        UserName = userName;
        FirstName = firstName;
        LastName = lastName;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string firstName, string lastName, string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required", nameof(lastName));

        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        Email = email.ToLowerInvariant();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetRefreshToken(string refreshToken, DateTime expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ClearRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
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

    public bool HasRole(string roleName)
    {
        return UserRoles.Any(ur => ur.Role?.Name == roleName && !ur.IsDeleted);
    }

    public bool HasPermission(Permission permission)
    {
        return UserRoles.Any(ur =>
            !ur.IsDeleted &&
            ur.Role != null &&
            ur.Role.IsActive &&
            ur.Role.RolePermissions.Any(rp =>
                rp.Permission == permission &&
                !rp.IsDeleted));
    }
}
