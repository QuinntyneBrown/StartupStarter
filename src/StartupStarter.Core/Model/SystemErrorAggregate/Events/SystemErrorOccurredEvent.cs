using StartupStarter.Core.Model.SystemErrorAggregate.Enums;

namespace StartupStarter.Core.Model.SystemErrorAggregate.Events;

public class SystemErrorOccurredEvent : DomainEvent
{
    public string ErrorId { get; set; } = string.Empty;
    public string ErrorType { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public ErrorSeverity Severity { get; set; }
    public int AffectedAccounts { get; set; }
    public DateTime Timestamp { get; set; }
}
