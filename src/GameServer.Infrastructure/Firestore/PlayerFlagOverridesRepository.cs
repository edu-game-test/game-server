using GameServer.Domain.Documents;
using Google.Cloud.Firestore;

namespace GameServer.Infrastructure.Firestore;

public sealed class PlayerFlagOverridesRepository
{
    private readonly FirestoreContext _ctx;
    public PlayerFlagOverridesRepository(FirestoreContext ctx) => _ctx = ctx;

    private DocumentReference Doc(string gameId, string playerId)
        => _ctx.Db.Document(FirestorePaths.PlayerSub(gameId, playerId, "overrides") + "/flags");

    public async Task<PlayerFlagOverridesDocument?> GetAsync(string gameId, string playerId, CancellationToken ct = default)
    {
        var snap = await Doc(gameId, playerId).GetSnapshotAsync(ct);
        return snap.Exists ? snap.ConvertTo<PlayerFlagOverridesDocument>() : null;
    }

    public Task SaveAsync(string gameId, string playerId, PlayerFlagOverridesDocument doc, CancellationToken ct = default)
        => Doc(gameId, playerId).SetAsync(doc, cancellationToken: ct);
}
