using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.API.Controllers;

/// <summary>
/// Manages turn-based battle sessions.
///
/// Battle is fully server-authoritative: the client submits player actions and
/// receives the computed outcome. The server owns all RNG seeds, damage formulas,
/// and state transitions. Clients must never trust locally computed results.
/// Each session is stored in Redis with a short TTL; expired sessions are
/// automatically abandoned and energy is refunded.
/// </summary>
[ApiController]
[Route("battle")]
[Authorize]
public class BattleController : ControllerBase
{
    /// <summary>
    /// Creates a new battle session for the authenticated player against the
    /// specified stage or opponent. Returns a session ID and the initial game state.
    /// </summary>
    [HttpPost("start")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartBattle([FromBody] StartBattleRequest request)
    {
        // TODO: validate hero team, deduct energy, seed RNG, persist session
        await Task.CompletedTask;
        var sessionId = Guid.NewGuid().ToString();
        return StatusCode(StatusCodes.Status201Created, new { sessionId, state = "initializing" });
    }

    /// <summary>
    /// Submits a player action for the current turn and returns the server-computed
    /// turn result, including damage dealt, status effects applied, and the updated
    /// game state.
    /// </summary>
    [HttpPost("turn")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SubmitTurn([FromBody] SubmitTurnRequest request)
    {
        // TODO: load session from Redis, validate action, compute turn, persist
        await Task.CompletedTask;
        return Ok(new { turnResult = "computed", sessionState = "in_progress" });
    }

    /// <summary>
    /// Returns the current snapshot of a battle session. Useful for reconnecting
    /// after a client disconnect.
    /// </summary>
    [HttpGet("{sessionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSession(string sessionId)
    {
        // TODO: load session from Redis, verify ownership
        await Task.CompletedTask;
        return Ok(new { sessionId, state = "in_progress" });
    }
}

public record StartBattleRequest(string StageId, string[] HeroIds);
public record SubmitTurnRequest(string SessionId, string ActionType, string? TargetId, string? SkillId);
