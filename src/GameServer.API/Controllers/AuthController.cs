using Microsoft.AspNetCore.Mvc;

namespace GameServer.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    /// <summary>
    /// Registers a new player account linked to a Firebase UID.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // TODO: validate Firebase ID token, create player record
        await Task.CompletedTask;
        return StatusCode(StatusCodes.Status201Created, new { playerId = Guid.NewGuid().ToString() });
    }

    /// <summary>
    /// Exchanges a Firebase ID token for a game session token.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // TODO: verify Firebase ID token, issue JWT
        await Task.CompletedTask;
        return Ok(new { accessToken = string.Empty, refreshToken = string.Empty, expiresIn = 3600 });
    }

    /// <summary>
    /// Issues a new access token from a valid refresh token.
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        // TODO: validate refresh token, issue new access token
        await Task.CompletedTask;
        return Ok(new { accessToken = string.Empty, expiresIn = 3600 });
    }
}

public record RegisterRequest(string FirebaseIdToken, string Username);
public record LoginRequest(string FirebaseIdToken);
public record RefreshRequest(string RefreshToken);
