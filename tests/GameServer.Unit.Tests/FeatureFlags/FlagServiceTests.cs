using GameServer.Application.FeatureFlags;
using MetaFramework.FeatureFlags;
using Moq;
using Xunit;

namespace GameServer.Unit.Tests.FeatureFlags;

public class FlagServiceTests
{
    [Fact]
    public async Task EvaluateAll_NoStoredFlagSet_UsesDefaultAndSeeds()
    {
        var repo = new Mock<IFlagSetRepository>();
        repo.Setup(r => r.GetAsync("match3", default)).ReturnsAsync((FlagSet?)null);
        repo.Setup(r => r.SaveAsync("match3", It.IsAny<FlagSet>(), default)).Returns(Task.CompletedTask);

        var svc = new FlagService(repo.Object);
        var input = new FlagEvaluationInput { UserId = "u1", Platform = "android" };
        var flags = await svc.EvaluateAllAsync("match3", input);

        Assert.NotEmpty(flags);
        Assert.True(flags.ContainsKey("new_player_tutorial"));
        repo.Verify(r => r.SaveAsync("match3", It.IsAny<FlagSet>(), default), Times.Once);
    }

    [Fact]
    public async Task EvaluateAll_StoredFlagSet_ReturnsEvaluatedValues()
    {
        var stored = new FlagSet
        {
            Version = 2,
            Flags = new List<FlagDefinition>
            {
                new FlagDefinition { Key = "my_flag", Type = FlagType.Boolean, DefaultValue = false }
            }
        };
        var repo = new Mock<IFlagSetRepository>();
        repo.Setup(r => r.GetAsync("match3", default)).ReturnsAsync(stored);

        var svc = new FlagService(repo.Object);
        var input = new FlagEvaluationInput { UserId = "u1", Platform = "android" };
        var flags = await svc.EvaluateAllAsync("match3", input);

        Assert.True(flags.ContainsKey("my_flag"));
        Assert.Equal(false, flags["my_flag"]);
    }

    [Fact]
    public async Task GetVersion_ReturnsStoredVersion()
    {
        var repo = new Mock<IFlagSetRepository>();
        repo.Setup(r => r.GetAsync("match3", default)).ReturnsAsync(new FlagSet { Version = 7, Flags = new List<FlagDefinition> { new FlagDefinition { Key = "x", DefaultValue = true } } });

        var svc = new FlagService(repo.Object);
        var version = await svc.GetVersionAsync("match3");

        Assert.Equal(7, version);
    }
}
