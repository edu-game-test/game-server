using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.API.Controllers;

[ApiController]
[Route("admin")]
[Authorize]
[Authorize(Policy = "AdminOnly")]
public class AdminController : ControllerBase
{
    /// <summary>
    /// Sends a server-side game event (e.g. bonus XP weekend, double-drop) to
    /// all online players or a targeted subset. Requires admin role.
    /// </summary>
    [HttpPost("events/send")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendEvent([FromBody] SendEventRequest request)
    {
        // TODO: validate event payload, publish to event bus
        await Task.CompletedTask;
        return Accepted(new { eventId = Guid.NewGuid().ToString(), status = "queued" });
    }

    /// <summary>
    /// Bans a player account by player ID. A banned player cannot authenticate
    /// or perform any game actions. Requires admin role.
    /// </summary>
    [HttpPost("players/{id}/ban")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BanPlayer(string id, [FromBody] BanPlayerRequest request)
    {
        // TODO: mark player as banned in DB, revoke active sessions in Redis
        await Task.CompletedTask;
        return Ok(new { playerId = id, banned = true, reason = request.Reason });
    }

    /// <summary>
    /// Broadcasts an in-game announcement to all connected players via push
    /// notification or in-game mailbox. Requires admin role.
    /// </summary>
    [HttpPost("announce")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Announce([FromBody] AnnounceRequest request)
    {
        // TODO: enqueue announcement for delivery via FCM / in-game mailbox
        await Task.CompletedTask;
        return Accepted(new { announcementId = Guid.NewGuid().ToString(), status = "queued" });
    }
}

public record SendEventRequest(string EventType, Dictionary<string, object> Payload, string? TargetSegment);
public record BanPlayerRequest(string Reason, DateTime? ExpiresAt);
public record AnnounceRequest(string Title, string Body, string? ImageUrl);
