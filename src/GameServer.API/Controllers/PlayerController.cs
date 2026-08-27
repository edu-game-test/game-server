using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.API.Controllers;

[ApiController]
[Route("player")]
[Authorize]
public class PlayerController : ControllerBase
{
    /// <summary>
    /// Returns the authenticated player's public profile including username,
    /// level, and currency balances.
    /// </summary>
    [HttpGet("profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile()
    {
        // TODO: resolve player ID from JWT claim, fetch from repository
        await Task.CompletedTask;
        return Ok(new
        {
            playerId = string.Empty,
            username = string.Empty,
            level = 1,
            gold = 0,
            gems = 0,
            summonShards = 0,
            energyPoints = 100
        });
    }

    /// <summary>
    /// Returns the player's XP, level thresholds, completed stages, and
    /// achievement progress.
    /// </summary>
    [HttpGet("progression")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProgression()
    {
        // TODO: fetch progression record, compute next-level XP threshold
        await Task.CompletedTask;
        return Ok(new
        {
            level = 1,
            experiencePoints = 0L,
            nextLevelXp = 1000L,
            completedStages = Array.Empty<string>(),
            achievements = Array.Empty<object>()
        });
    }
}
