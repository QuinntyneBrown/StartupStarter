namespace StartupStarter.Api.Features.ApiManagement.Dtos;

public class ApiRequestDto
{
    public string RequestId { get; set; } = string.Empty;
    public string ApiKeyId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public int ResponseStatus { get; set; }
    public DateTime RequestedAt { get; set; }
}
