using GameServer.Domain.Documents;
using MetaFramework.Segmentation;

namespace GameServer.Application.Segments;

public sealed class NullSegmentService : ISegmentService
{
    public Task<SegmentAssignment> AssignAsync(PlayerDocument player, string gameId, CancellationToken ct = default)
        => Task.FromResult(new SegmentAssignment
        {
            SegmentIds = new List<string>(),
            AbAssignments = new Dictionary<string, string>(),
            EvaluatedAt = DateTime.UtcNow,
            SegmentSetVersion = 0
        });
}
