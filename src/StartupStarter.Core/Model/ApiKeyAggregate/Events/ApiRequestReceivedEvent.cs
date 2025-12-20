using StartupStarter.Core.Model.ApiKeyAggregate.Enums;

namespace StartupStarter.Core.Model.ApiKeyAggregate.Events;

public class ApiRequestReceivedEvent : DomainEvent
{
    public string RequestId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public ApiHttpMethod Method { get; set; }
    public string ApiKeyId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
