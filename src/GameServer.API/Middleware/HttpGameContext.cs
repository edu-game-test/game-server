using GameServer.Domain;

namespace GameServer.API.Middleware;

public sealed class HttpGameContext : IGameContext
{
    public string GameId { get; set; } = string.Empty;
}
