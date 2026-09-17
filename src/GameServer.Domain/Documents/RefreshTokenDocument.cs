using Google.Cloud.Firestore;

namespace GameServer.Domain.Documents;

[FirestoreData]
public sealed class RefreshTokenDocument
{
    [FirestoreProperty] public string Id { get; set; } = string.Empty;
    [FirestoreProperty] public string PlayerId { get; set; } = string.Empty;
    [FirestoreProperty] public string GameId { get; set; } = string.Empty;
    [FirestoreProperty] public Timestamp ExpiresAt { get; set; }
    [FirestoreProperty] public bool Revoked { get; set; }
}
