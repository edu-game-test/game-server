using GameServer.Application.Segments;
using GameServer.Domain.Documents;
using MetaFramework.Segmentation;

namespace GameServer.Application.Segmentation;

public sealed class SegmentService : ISegmentService
{
    private readonly ISegmentSetRepository _repo;
    private readonly Dictionary<string, (SegmentSet Set, DateTime LoadedAt)> _cache = new();
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);
    private readonly SemaphoreSlim _lock = new(1, 1);

    public SegmentService(ISegmentSetRepository repo) => _repo = repo;

    public async Task<SegmentAssignment> AssignAsync(PlayerDocument player, string gameId, CancellationToken ct = default)
    {
        var segmentSet = await GetOrLoadAsync(gameId, ct);
        if (segmentSet == null)
            return new SegmentAssignment { EvaluatedAt = DateTime.UtcNow };

        var resolveField = FieldResolver.ForPlayer(player);
        var segmentIds = segmentSet.Segments
            .Where(s => PredicateEvaluator.MatchesAll(s, resolveField))
            .Select(s => s.SegmentId)
            .ToList();

        var segmentIdSet = new HashSet<string>(segmentIds);
        var abAssignments = segmentSet.SplitTests.ToDictionary(
            t => t.SplitTestId,
            t => SplitTestResolver.Resolve(t, player.Id, segmentIdSet.Contains, DateTime.UtcNow));

        return new SegmentAssignment
        {
            SegmentIds = segmentIds,
            AbAssignments = abAssignments,
            EvaluatedAt = DateTime.UtcNow,
            SegmentSetVersion = segmentSet.Version
        };
    }

    public void Invalidate(string gameId)
    {
        lock (_cache) { _cache.Remove(gameId); }
    }

    private async Task<SegmentSet?> GetOrLoadAsync(string gameId, CancellationToken ct)
    {
        await _lock.WaitAsync(ct);
        try
        {
            if (_cache.TryGetValue(gameId, out var cached) && DateTime.UtcNow - cached.LoadedAt < CacheTtl)
                return cached.Set;

            var set = await _repo.GetAsync(gameId, ct);
            if (set == null)
            {
                set = DefaultSegmentSets.Match3();
                await _repo.SaveAsync(gameId, set, ct);
            }

            _cache[gameId] = (set, DateTime.UtcNow);
            return set;
        }
        finally
        {
            _lock.Release();
        }
    }
}
