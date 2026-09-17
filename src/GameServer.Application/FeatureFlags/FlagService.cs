using GameServer.Domain.Repositories;
using MetaFramework.FeatureFlags;

namespace GameServer.Application.FeatureFlags;

public sealed class FlagService : IFlagService
{
    private readonly IFlagSetRepository _repo;
    private readonly Dictionary<string, (FlagSet FlagSet, DateTime LoadedAt)> _cache = new();
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);
    private readonly SemaphoreSlim _lock = new(1, 1);

    public FlagService(IFlagSetRepository repo) => _repo = repo;

    public async Task<Dictionary<string, object>> EvaluateAllAsync(string gameId, FlagEvaluationInput input, CancellationToken ct = default)
    {
        var flagSet = await GetOrLoadAsync(gameId, ct);
        if (flagSet == null) return new Dictionary<string, object>();
        return FlagEvaluator.ResolveAll(flagSet, input);
    }

    public async Task<int> GetVersionAsync(string gameId, CancellationToken ct = default)
    {
        var flagSet = await GetOrLoadAsync(gameId, ct);
        return flagSet?.Version ?? 0;
    }

    public async Task<FlagSet?> GetFlagSetAsync(string gameId, CancellationToken ct = default)
        => await GetOrLoadAsync(gameId, ct);

    public async Task SaveFlagSetAsync(string gameId, FlagSet flagSet, CancellationToken ct = default)
    {
        await _repo.SaveAsync(gameId, flagSet, ct);
        Invalidate(gameId);
    }

    public void Invalidate(string gameId)
    {
        lock (_cache) { _cache.Remove(gameId); }
    }

    private async Task<FlagSet?> GetOrLoadAsync(string gameId, CancellationToken ct)
    {
        await _lock.WaitAsync(ct);
        try
        {
            if (_cache.TryGetValue(gameId, out var cached) && DateTime.UtcNow - cached.LoadedAt < CacheTtl)
                return cached.FlagSet;

            var flagSet = await _repo.GetAsync(gameId, ct);
            if (flagSet == null)
            {
                flagSet = DefaultFlagSets.Match3();
                await _repo.SaveAsync(gameId, flagSet, ct);
            }

            _cache[gameId] = (flagSet, DateTime.UtcNow);
            return flagSet;
        }
        finally
        {
            _lock.Release();
        }
    }
}
