using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.API.Controllers;

[ApiController]
[Route("heroes")]
[Authorize]
public class HeroesController : ControllerBase
{
    /// <summary>
    /// Returns all heroes in the authenticated player's collection.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCollection()
    {
        // TODO: return player's hero roster
        await Task.CompletedTask;
        return Ok(Array.Empty<object>());
    }

    /// <summary>
    /// Returns a single hero by its instance ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHero(string id)
    {
        // TODO: fetch hero instance for the authenticated player
        await Task.CompletedTask;
        return Ok(new { id });
    }

    /// <summary>
    /// Upgrades the hero's level or skill using resources from the player's inventory.
    /// </summary>
    [HttpPost("{id}/upgrade")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpgradeHero(string id, [FromBody] UpgradeHeroRequest request)
    {
        // TODO: validate resources, apply upgrade, persist
        await Task.CompletedTask;
        return Ok(new { id, upgraded = true });
    }

    /// <summary>
    /// Equips or unequips a piece of gear on the specified hero.
    /// </summary>
    [HttpPost("{id}/equip")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EquipItem(string id, [FromBody] EquipItemRequest request)
    {
        // TODO: validate gear ownership, update loadout
        await Task.CompletedTask;
        return Ok(new { id, equipped = request.ItemId });
    }
}

public record UpgradeHeroRequest(string UpgradeType);
public record EquipItemRequest(string ItemId, string Slot);
