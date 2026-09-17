using GameServer.Application.FeatureFlags;
using GameServer.Domain;
using GameServer.Infrastructure.Firestore;
using MetaFramework.FeatureFlags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.API.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/v1/flags")]
public class FlagsController : ControllerBase
{
    private readonly IFlagService _flags;
    private readonly PlayerFlagOverridesRepository _overrides;

    public FlagsController(IFlagService flags, PlayerFlagOverridesRepository overrides)
    {
        _flags = flags;
        _overrides = overrides;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFlags(
        [FromQuery] string platform,
        [FromQuery] string clientVersion = "0.0.0",
        [FromServices] IGameContext gameContext = null!,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(platform))
            return BadRequest(new { error = "missing_platform", message = "platform query parameter is required." });

        IReadOnlyDictionary<string, object?>? userOverrides = null;
        var playerId = User.FindFirst("sub")?.Value;
        if (!string.IsNullOrEmpty(playerId))
        {
            var overrideDoc = await _overrides.GetAsync(gameContext.GameId, playerId, ct);
            if (overrideDoc?.Overrides?.Count > 0)
                userOverrides = overrideDoc.Overrides;
        }

        var input = new FlagEvaluationInput
        {
            UserId = playerId ?? "anonymous",
            Platform = platform,
            ClientVersion = clientVersion,
            Segments = new List<string>(),
            UserOverrides = userOverrides
        };

        var version = await _flags.GetVersionAsync(gameContext.GameId, ct);
        var flags = await _flags.EvaluateAllAsync(gameContext.GameId, input, ct);

        return Ok(new { version, flags });
    }
}
