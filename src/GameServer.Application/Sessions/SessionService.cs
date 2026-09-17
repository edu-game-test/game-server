using System.Security.Cryptography;
using System.Text;
using GameServer.Application.Segments;
using GameServer.Domain;
using GameServer.Domain.Documents;
using GameServer.Domain.Repositories;
using Google.Cloud.Firestore;
using MetaFramework.Session;
using Microsoft.Extensions.Options;

namespace GameServer.Application.Sessions;

public sealed class SessionService : ISessionService
{
    private readonly IFirebaseTokenVerifier _verifier;
    private readonly IJwtIssuer _jwt;
    private readonly IAccountRepository _accounts;
    private readonly IPlayerRepository _players;
    private readonly IRefreshTokenRepository _tokens;
    private readonly ISegmentService _segments;
    private readonly int _accessTokenMinutes;
    private readonly int _refreshTokenDays;

    public SessionService(
        IFirebaseTokenVerifier verifier,
        IJwtIssuer jwt,
        IAccountRepository accounts,
        IPlayerRepository players,
        IRefreshTokenRepository tokens,
        ISegmentService segments,
        IOptions<SessionOptions> opts)
    {
        _verifier = verifier;
        _jwt = jwt;
        _accounts = accounts;
        _players = players;
        _tokens = tokens;
        _segments = segments;
        _accessTokenMinutes = opts.Value.AccessTokenMinutes;
        _refreshTokenDays = opts.Value.RefreshTokenDays;
    }

    public async Task<SessionResponse> CreateAsync(CreateSessionRequest request, string gameId, CancellationToken ct = default)
    {
        var identity = await _verifier.VerifyAsync(request.FirebaseIdToken, ct);

        var account = await _accounts.GetAsync(identity.FirebaseUid, ct)
                      ?? new AccountDocument
                      {
                          FirebaseUid = identity.FirebaseUid,
                          Provider = identity.Provider,
                          CreatedAt = Timestamp.GetCurrentTimestamp()
                      };

        bool isNew = !account.Players.ContainsKey(gameId);
        string playerId;

        if (!isNew)
        {
            playerId = account.Players[gameId];
        }
        else
        {
            playerId = Guid.NewGuid().ToString("N");
            account.Players[gameId] = playerId;
        }

        await _accounts.UpsertAsync(account, ct);

        PlayerDocument player;
        if (isNew)
        {
            player = new PlayerDocument
            {
                Id = playerId,
                AccountId = account.FirebaseUid,
                DisplayName = identity.IsAnonymous ? "Guest" : identity.FirebaseUid,
                IsGuest = identity.IsAnonymous,
                Platform = request.Platform,
                Locale = request.Locale ?? "en",
                InstalledAt = Timestamp.GetCurrentTimestamp(),
                LastLoginAt = Timestamp.GetCurrentTimestamp()
            };
            TouchSessionDays(player);
            await _players.CreateAsync(gameId, player, ct);
        }
        else
        {
            player = await _players.GetAsync(gameId, playerId, ct)
                     ?? throw new DomainException(500, "player_not_found", "Player record is missing.");
            player.LastLoginAt = Timestamp.GetCurrentTimestamp();
            TouchSessionDays(player);
            await _players.UpdateAsync(gameId, player, ct);
        }

        var assignment = await _segments.AssignAsync(player, gameId, ct);
        var rawRefresh = GenerateRefreshToken(playerId);
        var hash = HashToken(rawRefresh);

        await _tokens.StoreAsync(gameId, playerId, new RefreshTokenDocument
        {
            Id = hash,
            PlayerId = playerId,
            GameId = gameId,
            ExpiresAt = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(_refreshTokenDays).ToUniversalTime()),
            Revoked = false
        }, ct);

        return new SessionResponse
        {
            AccessToken = _jwt.IssueAccessToken(new JwtClaims { Sub = playerId, GameId = gameId }),
            RefreshToken = rawRefresh,
            ExpiresInSeconds = _accessTokenMinutes * 60,
            PlayerId = playerId,
            IsNewPlayer = isNew,
            IsGuest = identity.IsAnonymous,
            Segments = assignment.SegmentIds,
            AbAssignments = assignment.AbAssignments,
            SegmentSetVersion = assignment.SegmentSetVersion,
            ServerTimeUtc = DateTime.UtcNow
        };
    }

    public async Task<SessionResponse> RefreshAsync(RefreshSessionRequest request, string gameId, CancellationToken ct = default)
    {
        var parts = request.RefreshToken.Split('.', 2);
        if (parts.Length != 2)
            throw new DomainException(401, "invalid_token", "Malformed refresh token.");

        var playerId = parts[0];
        var hash = HashToken(request.RefreshToken);

        var tokenDoc = await _tokens.GetByHashAsync(gameId, playerId, hash, ct);
        if (tokenDoc == null || tokenDoc.Revoked || tokenDoc.ExpiresAt.ToDateTime() < DateTime.UtcNow)
            throw new DomainException(401, "invalid_token", "Refresh token is invalid or expired.");

        await _tokens.RevokeAsync(gameId, playerId, hash, ct);

        var player = await _players.GetAsync(gameId, playerId, ct)
                     ?? throw new DomainException(404, "player_not_found", "Player not found.");
        player.LastLoginAt = Timestamp.GetCurrentTimestamp();
        TouchSessionDays(player);
        await _players.UpdateAsync(gameId, player, ct);

        var assignment = await _segments.AssignAsync(player, gameId, ct);
        var newRaw = GenerateRefreshToken(playerId);
        var newHash = HashToken(newRaw);

        await _tokens.StoreAsync(gameId, playerId, new RefreshTokenDocument
        {
            Id = newHash,
            PlayerId = playerId,
            GameId = gameId,
            ExpiresAt = tokenDoc.ExpiresAt,
            Revoked = false
        }, ct);

        return new SessionResponse
        {
            AccessToken = _jwt.IssueAccessToken(new JwtClaims { Sub = playerId, GameId = gameId }),
            RefreshToken = newRaw,
            ExpiresInSeconds = _accessTokenMinutes * 60,
            PlayerId = playerId,
            IsNewPlayer = false,
            IsGuest = player.IsGuest,
            Segments = assignment.SegmentIds,
            AbAssignments = assignment.AbAssignments,
            SegmentSetVersion = assignment.SegmentSetVersion,
            ServerTimeUtc = DateTime.UtcNow
        };
    }

    private static void TouchSessionDays(PlayerDocument player)
    {
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        if (!player.RecentSessionDays.Contains(today))
        {
            player.RecentSessionDays.Add(today);
            if (player.RecentSessionDays.Count > 7)
                player.RecentSessionDays.RemoveAt(0);
        }
    }

    private static string GenerateRefreshToken(string playerId)
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return $"{playerId}.{Convert.ToHexString(bytes).ToLowerInvariant()}";
    }

    internal static string HashToken(string raw)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
