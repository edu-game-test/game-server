using MetaFramework.FeatureFlags;

namespace GameServer.Application.FeatureFlags;

public interface IFlagService
{
    Task<Dictionary<string, object>> EvaluateAllAsync(string gameId, FlagEvaluationInput input, CancellationToken ct = default);
    Task<int> GetVersionAsync(string gameId, CancellationToken ct = default);
}
