using System.Text.Json;
using GameServer.Application.FeatureFlags;
using GameServer.Domain.Documents;
using GameServer.Application.FeatureFlags;
using Google.Cloud.Firestore;
using MetaFramework.FeatureFlags;

namespace GameServer.Infrastructure.Firestore;

public sealed class FlagSetRepository : IFlagSetRepository
{
    private readonly FirestoreContext _ctx;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public FlagSetRepository(FirestoreContext ctx) => _ctx = ctx;

    private DocumentReference Doc(string gameId)
        => _ctx.Db.Document(FirestorePaths.Config(gameId, "flags"));

    public async Task<FlagSet?> GetAsync(string gameId, CancellationToken ct = default)
    {
        var snap = await Doc(gameId).GetSnapshotAsync(ct);
        if (!snap.Exists) return null;
        var doc = snap.ConvertTo<ConfigDocument>();
        if (string.IsNullOrEmpty(doc.Json)) return null;
        var flagSet = JsonSerializer.Deserialize<FlagSet>(doc.Json, JsonOpts);
        return flagSet == null ? null : JsonElementConverter.UnwrapAll(flagSet);
    }

    public async Task SaveAsync(string gameId, FlagSet flagSet, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(flagSet, JsonOpts);
        var doc = new ConfigDocument
        {
            Name = "flags",
            Json = json,
            Version = flagSet.Version,
            UpdatedAt = Timestamp.GetCurrentTimestamp()
        };
        await Doc(gameId).SetAsync(doc, cancellationToken: ct);
    }
}
