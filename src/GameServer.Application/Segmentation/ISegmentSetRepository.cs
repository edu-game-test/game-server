using MetaFramework.Segmentation;

namespace GameServer.Application.Segmentation;

public interface ISegmentSetRepository
{
    Task<SegmentSet?> GetAsync(string gameId, CancellationToken ct = default);
    Task SaveAsync(string gameId, SegmentSet segmentSet, CancellationToken ct = default);
}
