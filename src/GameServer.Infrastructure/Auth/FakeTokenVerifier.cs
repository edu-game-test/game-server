using GameServer.Application.Sessions;
using GameServer.Domain;

namespace GameServer.Infrastructure.Auth;

/// <summary>Accepts "fake:{uid}" or "fake:{uid}:{provider}" tokens when Auth:AllowFakeTokens is true.</summary>
public sealed class FakeTokenVerifier : IFirebaseTokenVerifier
{
    public Task<VerifiedIdentity> VerifyAsync(string idToken, CancellationToken ct = default)
    {
        if (!idToken.StartsWith("fake:", StringComparison.Ordinal))
            throw new DomainException(401, "unauthorized", "Invalid token format.");

        var rest = idToken[5..];
        var parts = rest.Split(':', 2);
        var uid = parts[0];
        var provider = parts.Length > 1 ? parts[1] : "password";
        return Task.FromResult(new VerifiedIdentity(uid, provider, provider == "anonymous"));
    }
}
