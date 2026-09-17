using FirebaseAdmin.Auth;
using GameServer.Application.Sessions;

namespace GameServer.Infrastructure.Auth;

public sealed class FirebaseTokenVerifier : IFirebaseTokenVerifier
{
    public async Task<VerifiedIdentity> VerifyAsync(string idToken, CancellationToken ct = default)
    {
        var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken, ct);

        string provider = "unknown";
        bool isAnonymous = false;

        if (decoded.Claims.TryGetValue("firebase", out var fb) && fb is System.Collections.Generic.Dictionary<string, object> fbDict)
        {
            if (fbDict.TryGetValue("sign_in_provider", out var p))
            {
                provider = p?.ToString() ?? "unknown";
                isAnonymous = provider == "anonymous";
            }
        }

        return new VerifiedIdentity(decoded.Uid, provider, isAnonymous);
    }
}
