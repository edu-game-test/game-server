using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.API.Controllers;

[ApiController]
[Route("arena")]
[Authorize]
public class ArenaController : ControllerBase
{
    /// <summary>
    /// Finds a suitable PvP opponent for the authenticated player based on
    /// their arena rating. Returns the opponent's public profile and their
    /// defending hero lineup.
    /// </summary>
    [HttpGet("matchmaking")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> FindMatch()
    {
        // TODO: query arena rating bucket, return closest-ranked opponent snapshot
        await Task.CompletedTask;
        return Ok(new
        {
            opponentId = string.Empty,
            opponentUsername = string.Empty,
            opponentRating = 0,
            defenderHeroes = Array.Empty<object>()
        });
    }

    /// <summary>
    /// Initiates a ranked arena battle against a matched opponent.
    /// The battle is resolved server-side; rating changes are applied on completion.
    /// </summary>
    [HttpPost("battle")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> StartArenaBattle([FromBody] ArenaBattleRequest request)
    {
        // TODO: validate attacker team, resolve battle engine, update ratings
        await Task.CompletedTask;
        return Ok(new
        {
            result = "victory",
            ratingChange = 0,
            newRating = 0,
            battleLog = Array.Empty<object>()
        });
    }
}

public record ArenaBattleRequest(string OpponentId, string[] AttackerHeroIds);
