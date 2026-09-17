using Google.Cloud.Firestore;

namespace GameServer.Domain.Documents;

[FirestoreData]
public sealed class PlayerDocument
{
    [FirestoreDocumentId] public string Id { get; set; } = string.Empty;
    [FirestoreProperty] public string AccountId { get; set; } = string.Empty;
    [FirestoreProperty] public string DisplayName { get; set; } = string.Empty;
    [FirestoreProperty] public string AvatarId { get; set; } = "default";
    [FirestoreProperty] public bool IsGuest { get; set; }
    [FirestoreProperty] public string Platform { get; set; } = string.Empty;
    [FirestoreProperty] public string CountryCode { get; set; } = string.Empty;
    [FirestoreProperty] public string Locale { get; set; } = "en";
    [FirestoreProperty] public Timestamp InstalledAt { get; set; }
    [FirestoreProperty] public Timestamp LastLoginAt { get; set; }
    /// <summary>UTC dates (yyyy-MM-dd) of the last sessions; trimmed to 7 days. Source of session_count_7d.</summary>
    [FirestoreProperty] public List<string> RecentSessionDays { get; set; } = new();
    [FirestoreProperty] public double LifetimeSpendUsd { get; set; }
    [FirestoreProperty] public int MaxLevelReached { get; set; }
    [FirestoreProperty] public int StarsTotal { get; set; }
}
