using System.Net.Http;
using Xunit;

namespace GameServer.Integration.Tests.Fixtures;

/// <summary>Requires `docker compose -f docker-compose.emulator.yml up -d`. Clears all documents before each test class.</summary>
public sealed class FirestoreEmulatorFixture : IAsyncLifetime
{
    public const string ProjectId = "test-project";
    public string Host { get; } = Environment.GetEnvironmentVariable("FIRESTORE_EMULATOR_HOST") ?? "localhost:8681";

    public async Task InitializeAsync()
    {
        Environment.SetEnvironmentVariable("FIRESTORE_EMULATOR_HOST", Host);
        await ClearAsync();
    }

    public async Task ClearAsync()
    {
        using var http = new HttpClient();
        var url = $"http://{Host}/emulator/v1/projects/{ProjectId}/databases/(default)/documents";
        var response = await http.DeleteAsync(url);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                $"Firestore emulator not reachable at {Host} ({response.StatusCode}). " +
                "Start it with: docker compose -f docker-compose.emulator.yml up -d");
    }

    public Task DisposeAsync() => Task.CompletedTask;
}

[CollectionDefinition("firestore")]
public sealed class FirestoreCollection : ICollectionFixture<FirestoreEmulatorFixture> { }
