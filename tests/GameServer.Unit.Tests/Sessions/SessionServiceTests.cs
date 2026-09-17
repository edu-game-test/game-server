using System.Security.Cryptography;
using System.Text;
using GameServer.Application.Segments;
using GameServer.Application.Sessions;
using GameServer.Domain.Documents;
using GameServer.Domain.Repositories;
using GameServer.Infrastructure.Auth;
using MetaFramework.Session;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace GameServer.Unit.Tests.Sessions;

public class SessionServiceTests
{
    private static SessionService BuildService(
        IFirebaseTokenVerifier? verifier = null,
        IJwtIssuer? jwt = null,
        IAccountRepository? accounts = null,
        IPlayerRepository? players = null,
        IRefreshTokenRepository? tokens = null,
        ISegmentService? segments = null,
        SessionOptions? opts = null)
    {
        verifier ??= new FakeTokenVerifier();
        jwt ??= Mock.Of<IJwtIssuer>(j => j.IssueAccessToken(It.IsAny<JwtClaims>()) == "test-token");
        accounts ??= Mock.Of<IAccountRepository>();
        players ??= Mock.Of<IPlayerRepository>();
        tokens ??= Mock.Of<IRefreshTokenRepository>();
        segments ??= new NullSegmentService();
        opts ??= new SessionOptions { AccessTokenMinutes = 60, RefreshTokenDays = 30 };
        return new SessionService(verifier, jwt, accounts, players, tokens, segments, Options.Create(opts));
    }

    [Fact]
    public async Task CreateAsync_NewPlayer_ReturnsIsNewPlayerTrue()
    {
        var accounts = new Mock<IAccountRepository>();
        accounts.Setup(a => a.GetAsync("uid1", default)).ReturnsAsync((AccountDocument?)null);
        accounts.Setup(a => a.UpsertAsync(It.IsAny<AccountDocument>(), default)).Returns(Task.CompletedTask);

        var players = new Mock<IPlayerRepository>();
        players.Setup(p => p.CreateAsync("match3", It.IsAny<PlayerDocument>(), default)).Returns(Task.CompletedTask);

        var tokens = new Mock<IRefreshTokenRepository>();
        tokens.Setup(t => t.StoreAsync("match3", It.IsAny<string>(), It.IsAny<RefreshTokenDocument>(), default))
              .Returns(Task.CompletedTask);

        var svc = BuildService(accounts: accounts.Object, players: players.Object, tokens: tokens.Object);

        var result = await svc.CreateAsync(
            new CreateSessionRequest { FirebaseIdToken = "fake:uid1", Platform = "android" },
            "match3");

        Assert.True(result.IsNewPlayer);
        Assert.Equal("test-token", result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.Contains(".", result.RefreshToken);
    }

    [Fact]
    public async Task CreateAsync_ExistingPlayer_ReturnsIsNewPlayerFalse()
    {
        var existingPlayerId = "player123";
        var accounts = new Mock<IAccountRepository>();
        accounts.Setup(a => a.GetAsync("uid2", default)).ReturnsAsync(new AccountDocument
        {
            FirebaseUid = "uid2",
            Players = new Dictionary<string, string> { { "match3", existingPlayerId } }
        });
        accounts.Setup(a => a.UpsertAsync(It.IsAny<AccountDocument>(), default)).Returns(Task.CompletedTask);

        var players = new Mock<IPlayerRepository>();
        players.Setup(p => p.GetAsync("match3", existingPlayerId, default)).ReturnsAsync(new PlayerDocument
        {
            Id = existingPlayerId,
            IsGuest = false,
            Platform = "ios",
            RecentSessionDays = new List<string>()
        });
        players.Setup(p => p.UpdateAsync("match3", It.IsAny<PlayerDocument>(), default)).Returns(Task.CompletedTask);

        var tokens = new Mock<IRefreshTokenRepository>();
        tokens.Setup(t => t.StoreAsync("match3", existingPlayerId, It.IsAny<RefreshTokenDocument>(), default))
              .Returns(Task.CompletedTask);

        var svc = BuildService(accounts: accounts.Object, players: players.Object, tokens: tokens.Object);

        var result = await svc.CreateAsync(
            new CreateSessionRequest { FirebaseIdToken = "fake:uid2", Platform = "ios" },
            "match3");

        Assert.False(result.IsNewPlayer);
        Assert.Equal(existingPlayerId, result.PlayerId);
    }

    [Fact]
    public async Task RefreshAsync_ValidToken_RotatesAndReturnsNewTokens()
    {
        var playerId = "p1";
        var rawRefresh = $"{playerId}.{new string('a', 64)}";
        var hash = HashToken(rawRefresh);

        var players = new Mock<IPlayerRepository>();
        players.Setup(p => p.GetAsync("match3", playerId, default)).ReturnsAsync(new PlayerDocument
        {
            Id = playerId,
            IsGuest = false,
            RecentSessionDays = new List<string>()
        });
        players.Setup(p => p.UpdateAsync("match3", It.IsAny<PlayerDocument>(), default)).Returns(Task.CompletedTask);

        var tokens = new Mock<IRefreshTokenRepository>();
        tokens.Setup(t => t.GetByHashAsync("match3", playerId, hash, default)).ReturnsAsync(new RefreshTokenDocument
        {
            Id = hash,
            PlayerId = playerId,
            GameId = "match3",
            ExpiresAt = Google.Cloud.Firestore.Timestamp.FromDateTime(DateTime.UtcNow.AddDays(30)),
            Revoked = false
        });
        tokens.Setup(t => t.RevokeAsync("match3", playerId, hash, default)).Returns(Task.CompletedTask);
        tokens.Setup(t => t.StoreAsync("match3", playerId, It.IsAny<RefreshTokenDocument>(), default)).Returns(Task.CompletedTask);

        var svc = BuildService(players: players.Object, tokens: tokens.Object);

        var result = await svc.RefreshAsync(new RefreshSessionRequest { RefreshToken = rawRefresh }, "match3");

        Assert.Equal("test-token", result.AccessToken);
        Assert.NotEqual(rawRefresh, result.RefreshToken);
        tokens.Verify(t => t.RevokeAsync("match3", playerId, hash, default), Times.Once);
    }

    [Fact]
    public async Task RefreshAsync_RevokedToken_ThrowsDomainException()
    {
        var playerId = "p2";
        var rawRefresh = $"{playerId}.{new string('b', 64)}";
        var hash = HashToken(rawRefresh);

        var tokens = new Mock<IRefreshTokenRepository>();
        tokens.Setup(t => t.GetByHashAsync("match3", playerId, hash, default)).ReturnsAsync(new RefreshTokenDocument
        {
            Revoked = true,
            ExpiresAt = Google.Cloud.Firestore.Timestamp.FromDateTime(DateTime.UtcNow.AddDays(1))
        });

        var svc = BuildService(tokens: tokens.Object);

        await Assert.ThrowsAsync<GameServer.Domain.DomainException>(
            () => svc.RefreshAsync(new RefreshSessionRequest { RefreshToken = rawRefresh }, "match3"));
    }

    private static string HashToken(string raw)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
