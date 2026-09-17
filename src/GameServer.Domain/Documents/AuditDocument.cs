using Google.Cloud.Firestore;

namespace GameServer.Domain.Documents;

[FirestoreData]
public sealed class AuditDocument
{
    [FirestoreDocumentId] public string Id { get; set; } = string.Empty;
    [FirestoreProperty] public string Action { get; set; } = string.Empty;
    [FirestoreProperty] public string Actor { get; set; } = string.Empty;
    [FirestoreProperty] public string Details { get; set; } = string.Empty;
    [FirestoreProperty] public Timestamp OccurredAt { get; set; }
}
