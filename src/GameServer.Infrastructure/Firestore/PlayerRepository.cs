using GameServer.Domain.Documents;
using GameServer.Domain.Repositories;
using Google.Cloud.Firestore;

namespace GameServer.Infrastructure.Firestore;

public sealed class PlayerRepository : IPlayerRepository
{
    private readonly FirestoreDb _db;
    public PlayerRepository(FirestoreContext ctx) => _db = ctx.Db;

    public async Task<PlayerDocument?> GetAsync(string gameId, string playerId, CancellationToken ct = default)
    {
        var snap = await _db.Document(FirestorePaths.Player(gameId, playerId)).GetSnapshotAsync(ct);
        return snap.Exists ? snap.ConvertTo<PlayerDocument>() : null;
    }

    public Task CreateAsync(string gameId, PlayerDocument player, CancellationToken ct = default) =>
        _db.Document(FirestorePaths.Player(gameId, player.Id)).CreateAsync(player, ct);

    public Task UpdateAsync(string gameId, PlayerDocument player, CancellationToken ct = default) =>
        _db.Document(FirestorePaths.Player(gameId, player.Id)).SetAsync(player, SetOptions.Overwrite, ct);
}
