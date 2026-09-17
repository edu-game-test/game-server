using System.Net;
using GameServer.Integration.Tests.Fixtures;
using Xunit;

namespace GameServer.Integration.Tests;

[Collection("firestore")]
public class GameContextMiddlewareTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    public GameContextMiddlewareTests(ApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task MissingHeader_On_ApiRoute_Is400()
    {
        var response = await _client.GetAsync("/api/v1/flags?platform=android&clientVersion=1.0.0");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Health_NeedsNoHeader()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
