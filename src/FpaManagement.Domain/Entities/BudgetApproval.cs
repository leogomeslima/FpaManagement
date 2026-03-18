using FpaManagement.Domain.Common;
using FpaManagement.Domain.Enums;

namespace FpaManagement.Domain.Entities;

public class BudgetApproval : BaseAuditableEntity
{
    public Guid BudgetId
    {
        get; private set;
    }
    public Guid ApproverId
    {
        get; private set;
    }
    public ApprovalStatus Status
    {
        get; private set;
    }
    public string? Comments
    {
        get; private set;
    }
    public DateTime ApprovedAt
    {
        get; private set;
    }

    // Navigation properties
    public virtual Budget Budget { get; private set; } = null!;
    public virtual User Approver { get; private set; } = null!;

    private BudgetApproval()
    {
    } // For EF Core

    public BudgetApproval(Guid budgetId, Guid approverId, ApprovalStatus status, string? comments = null)
    {
        BudgetId = budgetId;
        ApproverId = approverId;
        Status = status;
        Comments = comments;
        ApprovedAt = DateTime.UtcNow;
    }
}
