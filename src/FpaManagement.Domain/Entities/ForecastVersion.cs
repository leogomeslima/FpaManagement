using FpaManagement.Domain.Common;
using FpaManagement.Domain.ValueObjects;

namespace FpaManagement.Domain.Entities;

public class ForecastVersion : BaseAuditableEntity
{
    public Guid ForecastId { get; private set; }
    public string VersionName { get; private set; }
    public string? Description { get; private set; }
    public int VersionNumber { get; private set; }
    public bool IsCurrent { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? ApprovedBy { get; private set; }

    public virtual Forecast Forecast { get; private set; } = null!;
    public virtual ICollection<ForecastItem> Items { get; private set; } = new List<ForecastItem>();

    private ForecastVersion() { }

    public ForecastVersion(Guid forecastId, string versionName, string? description, int versionNumber)
    {
        ForecastId = forecastId;
        VersionName = versionName;
        Description = description;
        VersionNumber = versionNumber;
        IsCurrent = versionNumber == 1;
    }

    public void AddItem(ForecastItem item)
    {
        Items.Add(item);
    }
    public void Approve(string approvedBy)
    {
        if (string.IsNullOrWhiteSpace(approvedBy))
            throw new ArgumentException("O aprovador deve ser informado.", nameof(approvedBy));

        ApprovedAt = DateTime.UtcNow;
        ApprovedBy = approvedBy;

        // TODO: Lógica para current apenas na aprovação
    }

    public Money CalculateTotal()
    {
        if (!Items.Any()) return new Money(0);
        return Items.Select(i => i.Amount).Aggregate((a, b) => a.Add(b));
    }
}
