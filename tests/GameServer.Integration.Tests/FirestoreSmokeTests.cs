using GameServer.Infrastructure.Firestore;
using GameServer.Integration.Tests.Fixtures;
using Xunit;

namespace GameServer.Integration.Tests;

[Collection("firestore")]
public class FirestoreSmokeTests
{
    [Fact]
    public async Task CanWriteAndReadDocument()
    {
        var ctx = new FirestoreContext(FirestoreEmulatorFixture.ProjectId);
        var doc = ctx.Db.Document(FirestorePaths.Player("match3", "p1"));
        await doc.SetAsync(new Dictionary<string, object> { ["displayName"] = "Smoke" });
        var snap = await doc.GetSnapshotAsync();
        Assert.Equal("Smoke", snap.GetValue<string>("displayName"));
    }
}
