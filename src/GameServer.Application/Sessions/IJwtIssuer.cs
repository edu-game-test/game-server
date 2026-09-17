namespace GameServer.Application.Sessions;

public sealed class JwtClaims
{
    public required string Sub { get; init; }
    public required string GameId { get; init; }
    public string Role { get; init; } = "player";
}

public interface IJwtIssuer
{
    string IssueAccessToken(JwtClaims claims);
}
