using GameServer.Application.FeatureFlags;
using GameServer.Domain;
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

    public FlagsController(IFlagService flags) => _flags = flags;

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

        var input = new FlagEvaluationInput
        {
            UserId = User.FindFirst("sub")?.Value ?? "anonymous",
            Platform = platform,
            ClientVersion = clientVersion,
            Segments = new List<string>(),
            UserOverrides = null
        };

        var version = await _flags.GetVersionAsync(gameContext.GameId, ct);
        var flags = await _flags.EvaluateAllAsync(gameContext.GameId, input, ct);

        return Ok(new { version, flags });
    }
}
