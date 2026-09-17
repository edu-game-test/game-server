using GameServer.Application.Sessions;
using GameServer.Domain;
using MetaFramework.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.API.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly ISessionService _sessions;

    public AuthController(ISessionService sessions) => _sessions = sessions;

    [HttpPost("session")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateSession(
        [FromBody] CreateSessionRequest request,
        [FromServices] IGameContext gameContext,
        CancellationToken ct)
    {
        var response = await _sessions.CreateAsync(request, gameContext.GameId, ct);
        return Ok(response);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshSessionRequest request,
        [FromServices] IGameContext gameContext,
        CancellationToken ct)
    {
        var response = await _sessions.RefreshAsync(request, gameContext.GameId, ct);
        return Ok(response);
    }
}
