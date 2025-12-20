using StartupStarter.Core.Model.SystemErrorAggregate.Enums;
using StartupStarter.Core.Model.SystemErrorAggregate.Events;

namespace StartupStarter.Core.Model.SystemErrorAggregate.Entities;

public class SystemError
{
    public string ErrorId { get; private set; }
    public string ErrorType { get; private set; }
    public string ErrorMessage { get; private set; }
    public string StackTrace { get; private set; }
    public ErrorSeverity Severity { get; private set; }
    public int AffectedAccounts { get; private set; }
    public DateTime OccurredAt { get; private set; }

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // EF Core constructor
    private SystemError()
    {
        ErrorId = string.Empty;
        ErrorType = string.Empty;
        ErrorMessage = string.Empty;
        StackTrace = string.Empty;
    }

    public SystemError(string errorId, string errorType, string errorMessage, string stackTrace,
        ErrorSeverity severity, int affectedAccounts)
    {
        if (string.IsNullOrWhiteSpace(errorId))
            throw new ArgumentException("Error ID cannot be empty", nameof(errorId));
        if (string.IsNullOrWhiteSpace(errorType))
            throw new ArgumentException("Error type cannot be empty", nameof(errorType));
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("Error message cannot be empty", nameof(errorMessage));
        if (affectedAccounts < 0)
            throw new ArgumentException("Affected accounts cannot be negative", nameof(affectedAccounts));

        ErrorId = errorId;
        ErrorType = errorType;
        ErrorMessage = errorMessage;
        StackTrace = stackTrace ?? string.Empty;
        Severity = severity;
        AffectedAccounts = affectedAccounts;
        OccurredAt = DateTime.UtcNow;

        AddDomainEvent(new SystemErrorOccurredEvent
        {
            ErrorId = ErrorId,
            ErrorType = ErrorType,
            ErrorMessage = ErrorMessage,
            StackTrace = StackTrace,
            Severity = Severity,
            AffectedAccounts = AffectedAccounts,
            Timestamp = OccurredAt
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
