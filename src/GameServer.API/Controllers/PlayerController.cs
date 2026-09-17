using GameServer.Domain;
using GameServer.Domain.Documents;
using GameServer.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/player")]
public class PlayerController : ControllerBase
{
    private readonly IPlayerRepository _players;

    public PlayerController(IPlayerRepository players) => _players = players;

    [HttpGet("me")]
    [ProducesResponseType(typeof(PlayerProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMe(
        [FromServices] IGameContext gameContext,
        CancellationToken ct)
    {
        var playerId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(playerId))
            return Unauthorized();

        var player = await _players.GetAsync(gameContext.GameId, playerId, ct);
        if (player == null)
            return NotFound(new { error = "player_not_found" });

        return Ok(new PlayerProfileDto(
            player.Id,
            player.DisplayName,
            player.AvatarId,
            player.IsGuest,
            player.Platform,
            player.Locale));
    }
}

public sealed record PlayerProfileDto(
    string PlayerId,
    string DisplayName,
    string AvatarId,
    bool IsGuest,
    string Platform,
    string Locale);
