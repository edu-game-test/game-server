using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.API.Controllers;

[ApiController]
[Route("summon")]
[Authorize]
public class SummonController : ControllerBase
{
    /// <summary>
    /// Performs a single summon using one summon shard. Returns the summoned hero's
    /// base data and rarity. Pity counter is incremented server-side.
    /// </summary>
    [HttpPost("single")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SummonSingle([FromBody] SummonRequest request)
    {
        // TODO: deduct shard, roll gacha table with pity, award hero, persist
        await Task.CompletedTask;
        return Ok(new { heroId = string.Empty, rarity = "Common", isNew = true });
    }

    /// <summary>
    /// Performs a 10x summon using ten summon shards. Guarantees at least one
    /// Rare or higher result as per the gacha rate-up rules. Returns an ordered
    /// list of summon results.
    /// </summary>
    [HttpPost("multi")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SummonMulti([FromBody] SummonRequest request)
    {
        // TODO: deduct 10 shards, roll 10 times with guaranteed rare, persist
        await Task.CompletedTask;
        return Ok(new { results = Array.Empty<object>() });
    }
}

public record SummonRequest(string BannerId);
