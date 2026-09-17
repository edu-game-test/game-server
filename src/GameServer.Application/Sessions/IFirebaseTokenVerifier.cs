namespace GameServer.Application.Sessions;

public interface IFirebaseTokenVerifier
{
    Task<VerifiedIdentity> VerifyAsync(string idToken, CancellationToken ct = default);
}
