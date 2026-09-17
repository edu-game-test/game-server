namespace GameServer.Application.Sessions;

public sealed record VerifiedIdentity(string FirebaseUid, string Provider, bool IsAnonymous);
