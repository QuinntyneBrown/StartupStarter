using System.Text.Json;
using StartupStarter.Core.Model.AuditAggregate.Enums;
using StartupStarter.Core.Model.AuditAggregate.Events;

namespace StartupStarter.Core.Model.AuditAggregate.Entities;

public class AuditExport
{
    public string ExportId { get; private set; }
    public string AccountId { get; private set; }
    public string RequestedBy { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public string FiltersJson { get; private set; }
    public FileFormat FileFormat { get; private set; }
    public ExportStatus Status { get; private set; }
    public int RecordCount { get; private set; }
    public string FileLocation { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // EF Core constructor
    private AuditExport()
    {
        ExportId = string.Empty;
        AccountId = string.Empty;
        RequestedBy = string.Empty;
        FiltersJson = string.Empty;
        FileLocation = string.Empty;
    }

    public AuditExport(string exportId, string accountId, string requestedBy, DateTime startDate,
        DateTime endDate, Dictionary<string, object> filters, FileFormat fileFormat)
    {
        if (string.IsNullOrWhiteSpace(exportId))
            throw new ArgumentException("Export ID cannot be empty", nameof(exportId));
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("Account ID cannot be empty", nameof(accountId));
        if (string.IsNullOrWhiteSpace(requestedBy))
            throw new ArgumentException("RequestedBy cannot be empty", nameof(requestedBy));

        ExportId = exportId;
        AccountId = accountId;
        RequestedBy = requestedBy;
        StartDate = startDate;
        EndDate = endDate;
        FiltersJson = filters != null ? JsonSerializer.Serialize(filters) : string.Empty;
        FileFormat = fileFormat;
        Status = ExportStatus.Requested;
        RecordCount = 0;
        FileLocation = string.Empty;
        RequestedAt = DateTime.UtcNow;

        AddDomainEvent(new AuditExportRequestedEvent
        {
            ExportId = ExportId,
            AccountId = AccountId,
            RequestedBy = RequestedBy,
            DateRange = new { StartDate = startDate, EndDate = endDate },
            Filters = filters ?? new Dictionary<string, object>(),
            Timestamp = RequestedAt
        });
    }

    public void MarkCompleted(int recordCount, string fileLocation)
    {
        if (string.IsNullOrWhiteSpace(fileLocation))
            throw new ArgumentException("File location cannot be empty", nameof(fileLocation));

        Status = ExportStatus.Completed;
        RecordCount = recordCount;
        FileLocation = fileLocation;
        CompletedAt = DateTime.UtcNow;

        AddDomainEvent(new AuditExportCompletedEvent
        {
            ExportId = ExportId,
            AccountId = AccountId,
            FileFormat = FileFormat,
            RecordCount = recordCount,
            FileLocation = fileLocation,
            Timestamp = CompletedAt.Value
        });
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
