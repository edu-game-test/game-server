using MetaFramework.FeatureFlags;

namespace GameServer.Application.FeatureFlags;

public interface IFlagSetRepository
{
    Task<FlagSet?> GetAsync(string gameId, CancellationToken ct = default);
    Task SaveAsync(string gameId, FlagSet flagSet, CancellationToken ct = default);
}
