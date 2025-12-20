using MediatR;
using StartupStarter.Api.Features.ApiManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.ApiKeyAggregate.Entities;

namespace StartupStarter.Api.Features.ApiManagement.Commands;

public class CreateApiKeyCommandHandler : IRequestHandler<CreateApiKeyCommand, ApiKeyDto>
{
    private readonly IStartupStarterContext _context;

    public CreateApiKeyCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<ApiKeyDto> Handle(CreateApiKeyCommand request, CancellationToken cancellationToken)
    {
        var apiKeyId = Guid.NewGuid().ToString();

        var apiKey = new ApiKey(
            apiKeyId,
            request.KeyName,
            request.AccountId,
            request.KeyHash,
            request.CreatedBy,
            request.Permissions,
            request.ExpiresAt
        );

        _context.ApiKeys.Add(apiKey);
        await _context.SaveChangesAsync(cancellationToken);

        return apiKey.ToDto();
    }
}
