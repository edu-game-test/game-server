using GameServer.Domain.Documents;

namespace GameServer.Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenDocument?> GetByHashAsync(string gameId, string playerId, string tokenHash, CancellationToken ct = default);
    Task StoreAsync(string gameId, string playerId, RefreshTokenDocument document, CancellationToken ct = default);
    Task RevokeAsync(string gameId, string playerId, string tokenHash, CancellationToken ct = default);
}
