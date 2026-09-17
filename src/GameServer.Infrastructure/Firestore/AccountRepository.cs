using GameServer.Domain.Documents;
using GameServer.Domain.Repositories;
using Google.Cloud.Firestore;

namespace GameServer.Infrastructure.Firestore;

public sealed class AccountRepository : IAccountRepository
{
    private readonly FirestoreDb _db;
    public AccountRepository(FirestoreContext ctx) => _db = ctx.Db;

    public async Task<AccountDocument?> GetAsync(string firebaseUid, CancellationToken ct = default)
    {
        var snap = await _db.Document(FirestorePaths.Account(firebaseUid)).GetSnapshotAsync(ct);
        return snap.Exists ? snap.ConvertTo<AccountDocument>() : null;
    }

    public Task UpsertAsync(AccountDocument account, CancellationToken ct = default) =>
        _db.Document(FirestorePaths.Account(account.FirebaseUid)).SetAsync(account, SetOptions.MergeAll, ct);
}
