namespace GameServer.Application.Sessions;

public sealed class SessionOptions
{
    public int AccessTokenMinutes { get; set; } = 60;
    public int RefreshTokenDays { get; set; } = 30;
}
