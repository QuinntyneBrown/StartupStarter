using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StartupStarter.Api.Features.AuthManagement.Commands;
using StartupStarter.Api.Features.AuthManagement.Dtos;

namespace StartupStarter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginDto>> Login([FromBody] LoginCommand command)
    {
        // Add IP address and user agent from request
        command.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        command.UserAgent = HttpContext.Request.Headers["User-Agent"].ToString();

        var result = await _mediator.Send(command);

        if (result == null)
            return Unauthorized(new { message = "Invalid email or password" });

        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        // Get user ID from claims
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        // In a real implementation, you would end the session here
        // For now, just return success since JWT tokens are stateless

        return Ok(new { message = "Logged out successfully" });
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var name = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var accountId = User.FindFirst("AccountId")?.Value;
        var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();

        return Ok(new
        {
            userId,
            email,
            name,
            accountId,
            roles
        });
    }
}
