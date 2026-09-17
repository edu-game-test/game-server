using Google.Cloud.Firestore;

namespace GameServer.Domain.Documents;

[FirestoreData]
public sealed class PlayerFlagOverridesDocument
{
    [FirestoreDocumentId] public string PlayerId { get; set; } = string.Empty;
    [FirestoreProperty] public Dictionary<string, object?> Overrides { get; set; } = new();
    [FirestoreProperty] public Timestamp UpdatedAt { get; set; }
}
