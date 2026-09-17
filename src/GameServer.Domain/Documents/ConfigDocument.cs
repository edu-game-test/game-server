using Google.Cloud.Firestore;

namespace GameServer.Domain.Documents;

[FirestoreData]
public sealed class ConfigDocument
{
    [FirestoreDocumentId] public string Name { get; set; } = string.Empty;
    [FirestoreProperty] public string Json { get; set; } = string.Empty;
    [FirestoreProperty] public int Version { get; set; }
    [FirestoreProperty] public Timestamp UpdatedAt { get; set; }
}
