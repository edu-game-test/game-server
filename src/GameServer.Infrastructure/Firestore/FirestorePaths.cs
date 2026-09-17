namespace GameServer.Infrastructure.Firestore;

/// <summary>Single place that knows the games/{gameId}/... layout (master plan §3.1).</summary>
public static class FirestorePaths
{
    public static string Game(string gameId) => $"games/{gameId}";
    public static string Players(string gameId) => $"games/{gameId}/players";
    public static string Player(string gameId, string playerId) => $"games/{gameId}/players/{playerId}";
    public static string PlayerSub(string gameId, string playerId, string sub) => $"games/{gameId}/players/{playerId}/{sub}";
    public static string Config(string gameId, string name) => $"games/{gameId}/config/{name}";
    public static string ConfigHistory(string gameId, string name) => $"games/{gameId}/config/{name}History";
    public static string Accounts => "accounts";
    public static string Account(string firebaseUid) => $"accounts/{firebaseUid}";
    public static string Audit(string gameId) => $"games/{gameId}/audit";
}
