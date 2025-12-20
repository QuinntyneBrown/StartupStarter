using StartupStarter.Core.Model.ProfileAggregate.Enums;

namespace StartupStarter.Core.Model.ProfileAggregate.Events;

public class ProfileCreatedEvent : DomainEvent
{
    public string ProfileId { get; set; } = string.Empty;
    public string ProfileName { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public ProfileType ProfileType { get; set; }
    public bool IsDefault { get; set; }
    public DateTime Timestamp { get; set; }
}
