using MediatR;
using StartupStarter.Api.Features.SystemManagement.Dtos;

namespace StartupStarter.Api.Features.SystemManagement.Queries;

public class GetSystemErrorsQuery : IRequest<List<SystemErrorDto>>
{
}
