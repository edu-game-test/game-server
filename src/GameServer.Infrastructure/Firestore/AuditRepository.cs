using GameServer.Domain.Documents;
using Google.Cloud.Firestore;

namespace GameServer.Infrastructure.Firestore;

public sealed class AuditRepository
{
    private readonly FirestoreContext _ctx;
    public AuditRepository(FirestoreContext ctx) => _ctx = ctx;

    public Task AppendAsync(string gameId, AuditDocument entry, CancellationToken ct = default)
    {
        var docRef = _ctx.Db.Collection(FirestorePaths.Audit(gameId)).Document(entry.Id);
        return docRef.SetAsync(entry, cancellationToken: ct);
    }
}
