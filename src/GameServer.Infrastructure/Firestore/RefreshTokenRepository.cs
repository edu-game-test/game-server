using GameServer.Domain.Documents;
using GameServer.Domain.Repositories;

namespace GameServer.Infrastructure.Firestore;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly FirestoreContext _ctx;
    public RefreshTokenRepository(FirestoreContext ctx) => _ctx = ctx;

    private Google.Cloud.Firestore.CollectionReference Sessions(string gameId, string playerId)
        => _ctx.Db.Collection(FirestorePaths.PlayerSub(gameId, playerId, "sessions"));

    public async Task<RefreshTokenDocument?> GetByHashAsync(string gameId, string playerId, string tokenHash, CancellationToken ct = default)
    {
        var snap = await Sessions(gameId, playerId).Document(tokenHash).GetSnapshotAsync(ct);
        return snap.Exists ? snap.ConvertTo<RefreshTokenDocument>() : null;
    }

    public Task StoreAsync(string gameId, string playerId, RefreshTokenDocument document, CancellationToken ct = default)
        => Sessions(gameId, playerId).Document(document.Id).SetAsync(document, cancellationToken: ct);

    public Task RevokeAsync(string gameId, string playerId, string tokenHash, CancellationToken ct = default)
        => Sessions(gameId, playerId).Document(tokenHash)
            .UpdateAsync(new Dictionary<string, object> { { "Revoked", true } }, cancellationToken: ct);
}
