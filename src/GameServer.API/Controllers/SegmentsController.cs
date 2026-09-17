using GameServer.Application.Segments;
using GameServer.Domain;
using GameServer.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/segments")]
public class SegmentsController : ControllerBase
{
    private readonly ISegmentService _segments;
    private readonly IPlayerRepository _players;

    public SegmentsController(ISegmentService segments, IPlayerRepository players)
    {
        _segments = segments;
        _players = players;
    }

    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMySegments(
        [FromServices] IGameContext gameContext,
        CancellationToken ct)
    {
        var playerId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(playerId)) return Unauthorized();

        var player = await _players.GetAsync(gameContext.GameId, playerId, ct);
        if (player == null) return NotFound(new { error = "player_not_found" });

        var assignment = await _segments.AssignAsync(player, gameContext.GameId, ct);
        return Ok(assignment);
    }
}
