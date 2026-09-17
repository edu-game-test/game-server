using System.Text.Json;

namespace GameServer.API.Middleware;

/// <summary>Resolves the game id from the JWT claim (preferred) or the X-Game-Id header and validates it against Games:Allowed.</summary>
public sealed class GameContextMiddleware
{
    public const string Header = "X-Game-Id";
    private readonly RequestDelegate _next;
    private readonly HashSet<string> _allowed;

    public GameContextMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _allowed = configuration.GetSection("Games:Allowed").Get<string[]>()?.ToHashSet() ?? new HashSet<string>();
    }

    public async Task InvokeAsync(HttpContext http, HttpGameContext gameContext)
    {
        if (http.Request.Path.StartsWithSegments("/health") || http.Request.Path.StartsWithSegments("/swagger") || http.Request.Path.StartsWithSegments("/api/v1/admin"))
        {
            await _next(http);
            return;
        }

        var gameId = http.User.FindFirst("game_id")?.Value
                     ?? http.Request.Headers[Header].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(gameId) || !_allowed.Contains(gameId))
        {
            http.Response.StatusCode = StatusCodes.Status400BadRequest;
            http.Response.ContentType = "application/json";
            await http.Response.WriteAsync(JsonSerializer.Serialize(new { error = "unknown_game", message = $"Missing or unknown {Header}." }));
            return;
        }

        gameContext.GameId = gameId;
        await _next(http);
    }
}
