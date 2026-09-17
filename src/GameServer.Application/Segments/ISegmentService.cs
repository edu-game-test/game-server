using GameServer.Domain.Documents;
using MetaFramework.Segmentation;

namespace GameServer.Application.Segments;

public interface ISegmentService
{
    Task<SegmentAssignment> AssignAsync(PlayerDocument player, string gameId, CancellationToken ct = default);
}
