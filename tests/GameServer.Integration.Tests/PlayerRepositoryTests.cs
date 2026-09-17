using GameServer.Domain.Documents;
using GameServer.Infrastructure.Firestore;
using GameServer.Integration.Tests.Fixtures;
using Google.Cloud.Firestore;
using Xunit;

namespace GameServer.Integration.Tests;

[Collection("firestore")]
public class PlayerRepositoryTests
{
    [Fact]
    public async Task Create_Get_Update_RoundTrip()
    {
        var repo = new PlayerRepository(new FirestoreContext(FirestoreEmulatorFixture.ProjectId));
        var player = new PlayerDocument
        {
            Id = Guid.NewGuid().ToString("N"),
            DisplayName = "Guest",
            IsGuest = true,
            Platform = "android",
            InstalledAt = Timestamp.GetCurrentTimestamp(),
            LastLoginAt = Timestamp.GetCurrentTimestamp(),
        };

        await repo.CreateAsync("match3", player);

        var loaded = await repo.GetAsync("match3", player.Id);
        Assert.NotNull(loaded);
        Assert.True(loaded!.IsGuest);

        loaded.MaxLevelReached = 12;
        await repo.UpdateAsync("match3", loaded);
        Assert.Equal(12, (await repo.GetAsync("match3", player.Id))!.MaxLevelReached);
        Assert.Null(await repo.GetAsync("puzzle2", player.Id));
    }
}
