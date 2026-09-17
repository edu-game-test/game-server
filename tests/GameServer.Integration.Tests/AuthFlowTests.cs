using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GameServer.Integration.Tests.Fixtures;
using MetaFramework.Session;
using Xunit;

namespace GameServer.Integration.Tests;

[Collection("firestore")]
public class AuthFlowTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly ApiFactory _factory;

    public AuthFlowTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add("X-Game-Id", "match3");
    }

    [Fact]
    public async Task CreateSession_NewPlayer_Returns200WithTokens()
    {
        var body = new CreateSessionRequest
        {
            FirebaseIdToken = $"fake:uid-{Guid.NewGuid():N}",
            Platform = "android"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/session", body);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var session = await response.Content.ReadFromJsonAsync<SessionResponse>();
        Assert.NotNull(session);
        Assert.NotEmpty(session!.AccessToken);
        Assert.NotEmpty(session.RefreshToken);
        Assert.True(session.IsNewPlayer);
        Assert.NotEmpty(session.PlayerId);
    }

    [Fact]
    public async Task CreateSession_Twice_SameUid_ReturnsExistingPlayer()
    {
        var uid = $"uid-{Guid.NewGuid():N}";
        var body = new CreateSessionRequest { FirebaseIdToken = $"fake:{uid}", Platform = "ios" };

        var r1 = await _client.PostAsJsonAsync("/api/v1/auth/session", body);
        r1.EnsureSuccessStatusCode();
        var s1 = await r1.Content.ReadFromJsonAsync<SessionResponse>();

        var r2 = await _client.PostAsJsonAsync("/api/v1/auth/session", body);
        r2.EnsureSuccessStatusCode();
        var s2 = await r2.Content.ReadFromJsonAsync<SessionResponse>();

        Assert.Equal(s1!.PlayerId, s2!.PlayerId);
        Assert.False(s2.IsNewPlayer);
    }

    [Fact]
    public async Task RefreshSession_ValidToken_Returns200WithNewTokens()
    {
        var body = new CreateSessionRequest { FirebaseIdToken = $"fake:uid-{Guid.NewGuid():N}", Platform = "steam" };
        var createResp = await _client.PostAsJsonAsync("/api/v1/auth/session", body);
        var session = await createResp.Content.ReadFromJsonAsync<SessionResponse>();

        var refreshBody = new RefreshSessionRequest { RefreshToken = session!.RefreshToken };
        var refreshResp = await _client.PostAsJsonAsync("/api/v1/auth/refresh", refreshBody);
        Assert.Equal(HttpStatusCode.OK, refreshResp.StatusCode);

        var refreshed = await refreshResp.Content.ReadFromJsonAsync<SessionResponse>();
        Assert.NotNull(refreshed);
        Assert.NotEmpty(refreshed!.AccessToken);
        Assert.NotEqual(session.RefreshToken, refreshed.RefreshToken);
    }

    [Fact]
    public async Task GetPlayerMe_WithValidToken_Returns200()
    {
        var body = new CreateSessionRequest { FirebaseIdToken = $"fake:uid-{Guid.NewGuid():N}", Platform = "web" };
        var createResp = await _client.PostAsJsonAsync("/api/v1/auth/session", body);
        var session = await createResp.Content.ReadFromJsonAsync<SessionResponse>();

        var client2 = _factory.CreateClient();
        client2.DefaultRequestHeaders.Add("X-Game-Id", "match3");
        client2.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", session!.AccessToken);

        var meResp = await client2.GetAsync("/api/v1/player/me");
        Assert.Equal(HttpStatusCode.OK, meResp.StatusCode);

        var profile = await meResp.Content.ReadFromJsonAsync<PlayerProfileResponse>();
        Assert.Equal(session.PlayerId, profile!.PlayerId);
    }

    [Fact]
    public async Task GetPlayerMe_WithoutToken_Returns401()
    {
        var resp = await _client.GetAsync("/api/v1/player/me");
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }
}

internal sealed record PlayerProfileResponse(
    string PlayerId, string DisplayName, string AvatarId,
    bool IsGuest, string Platform, string Locale);
