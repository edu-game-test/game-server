using Google.Cloud.Firestore;

namespace GameServer.Domain.Documents;

[FirestoreData]
public sealed class AccountDocument
{
    [FirestoreDocumentId] public string FirebaseUid { get; set; } = string.Empty;
    /// <summary>gameId → playerId (overview.md §7 isolation; one account, one player per game).</summary>
    [FirestoreProperty] public Dictionary<string, string> Players { get; set; } = new();
    /// <summary>anonymous | google.com | apple.com | facebook.com | password</summary>
    [FirestoreProperty] public string Provider { get; set; } = "anonymous";
    [FirestoreProperty] public Timestamp CreatedAt { get; set; }
}
