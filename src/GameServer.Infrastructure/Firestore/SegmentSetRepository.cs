using System.Text.Json;
using GameServer.Application.Segmentation;
using GameServer.Domain.Documents;
using Google.Cloud.Firestore;
using MetaFramework.Segmentation;

namespace GameServer.Infrastructure.Firestore;

public sealed class SegmentSetRepository : ISegmentSetRepository
{
    private readonly FirestoreContext _ctx;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public SegmentSetRepository(FirestoreContext ctx) => _ctx = ctx;

    private DocumentReference Doc(string gameId)
        => _ctx.Db.Document(FirestorePaths.Config(gameId, "segments"));

    public async Task<SegmentSet?> GetAsync(string gameId, CancellationToken ct = default)
    {
        var snap = await Doc(gameId).GetSnapshotAsync(ct);
        if (!snap.Exists) return null;
        var doc = snap.ConvertTo<ConfigDocument>();
        return string.IsNullOrEmpty(doc.Json)
            ? null
            : JsonSerializer.Deserialize<SegmentSet>(doc.Json, JsonOpts);
    }

    public async Task SaveAsync(string gameId, SegmentSet segmentSet, CancellationToken ct = default)
    {
        var doc = new ConfigDocument
        {
            Name = "segments",
            Json = JsonSerializer.Serialize(segmentSet, JsonOpts),
            Version = segmentSet.Version,
            UpdatedAt = Timestamp.GetCurrentTimestamp()
        };
        await Doc(gameId).SetAsync(doc, cancellationToken: ct);
    }
}
