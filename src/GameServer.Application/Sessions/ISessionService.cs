using MetaFramework.Session;

namespace GameServer.Application.Sessions;

public interface ISessionService
{
    Task<SessionResponse> CreateAsync(CreateSessionRequest request, string gameId, CancellationToken ct = default);
    Task<SessionResponse> RefreshAsync(RefreshSessionRequest request, string gameId, CancellationToken ct = default);
}
