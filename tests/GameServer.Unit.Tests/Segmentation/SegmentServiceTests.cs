using GameServer.Application.Segmentation;
using GameServer.Application.Segments;
using GameServer.Domain.Documents;
using Google.Cloud.Firestore;
using MetaFramework.Segmentation;
using Moq;
using Xunit;

namespace GameServer.Unit.Tests.Segmentation;

public class SegmentServiceTests
{
    private static PlayerDocument MakePlayer(string id = "p1", int level = 0, int sessions = 0, double spend = 0.0)
        => new PlayerDocument
        {
            Id = id,
            MaxLevelReached = level,
            StarsTotal = 0,
            RecentSessionDays = Enumerable.Range(0, sessions).Select(i => DateTime.UtcNow.AddDays(-i).ToString("yyyy-MM-dd")).ToList(),
            LifetimeSpendUsd = spend,
            InstalledAt = Timestamp.GetCurrentTimestamp(),
            LastLoginAt = Timestamp.GetCurrentTimestamp()
        };

    [Fact]
    public async Task NewPlayer_WithLowLevel_IsInNewPlayerSegment()
    {
        var segSet = DefaultSegmentSets.Match3();
        var repo = new Mock<ISegmentSetRepository>();
        repo.Setup(r => r.GetAsync("match3", default)).ReturnsAsync(segSet);

        var svc = new SegmentService(repo.Object);
        var assignment = await svc.AssignAsync(MakePlayer(level: 3), "match3");

        Assert.Contains("new_player", assignment.SegmentIds);
    }

    [Fact]
    public async Task HighLevelPlayer_IsNotInNewPlayerSegment()
    {
        var segSet = DefaultSegmentSets.Match3();
        var repo = new Mock<ISegmentSetRepository>();
        repo.Setup(r => r.GetAsync("match3", default)).ReturnsAsync(segSet);

        var svc = new SegmentService(repo.Object);
        var assignment = await svc.AssignAsync(MakePlayer(level: 50), "match3");

        Assert.DoesNotContain("new_player", assignment.SegmentIds);
    }

    [Fact]
    public async Task SpenderPlayer_IsInSpenderSegment()
    {
        var segSet = DefaultSegmentSets.Match3();
        var repo = new Mock<ISegmentSetRepository>();
        repo.Setup(r => r.GetAsync("match3", default)).ReturnsAsync(segSet);

        var svc = new SegmentService(repo.Object);
        var assignment = await svc.AssignAsync(MakePlayer(spend: 9.99), "match3");

        Assert.Contains("spender", assignment.SegmentIds);
    }

    [Fact]
    public async Task NoStoredSegmentSet_SeedsDefaultAndReturnsAssignment()
    {
        var repo = new Mock<ISegmentSetRepository>();
        repo.Setup(r => r.GetAsync("match3", default)).ReturnsAsync((SegmentSet?)null);
        repo.Setup(r => r.SaveAsync("match3", It.IsAny<SegmentSet>(), default)).Returns(Task.CompletedTask);

        var svc = new SegmentService(repo.Object);
        var assignment = await svc.AssignAsync(MakePlayer(level: 1), "match3");

        Assert.NotNull(assignment);
        repo.Verify(r => r.SaveAsync("match3", It.IsAny<SegmentSet>(), default), Times.Once);
    }
}
