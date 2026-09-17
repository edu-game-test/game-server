using GameServer.Domain.Documents;

namespace GameServer.Domain.Repositories;

public interface IPlayerRepository
{
    Task<PlayerDocument?> GetAsync(string gameId, string playerId, CancellationToken ct = default);
    Task CreateAsync(string gameId, PlayerDocument player, CancellationToken ct = default);
    Task UpdateAsync(string gameId, PlayerDocument player, CancellationToken ct = default);
}
