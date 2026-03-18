using FpaManagement.Domain.Common;

namespace FpaManagement.Domain.Entities;

public class Department : BaseAuditableEntity
{
    public string Code
    {
        get; private set;
    }
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
    public Guid? ManagerId
    {
        get; private set;
    }

    // Navigation properties
    public virtual ICollection<CostCenter> CostCenters { get; private set; } = new List<CostCenter>();
    public virtual User? Manager
    {
        get; private set;
    }

    private Department()
    {
    } // For EF Core

    public Department(string code, string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Department code is required", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Department name is required", nameof(name));

        Code = code.ToUpperInvariant();
        Name = name;
        Description = description;
        IsActive = true;
    }

    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Department name is required", nameof(name));

        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetManager(Guid managerId)
    {
        ManagerId = managerId;
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

    public void AddCostCenter(CostCenter costCenter)
    {
        if (costCenter == null)
            throw new ArgumentNullException(nameof(costCenter));

        if (!CostCenters.Any(cc => cc.Code == costCenter.Code))
        {
            CostCenters.Add(costCenter);
        }
    }
}
