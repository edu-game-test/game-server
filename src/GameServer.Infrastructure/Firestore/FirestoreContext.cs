using Google.Api.Gax;
using Google.Cloud.Firestore;

namespace GameServer.Infrastructure.Firestore;

public sealed class FirestoreContext
{
    public FirestoreContext(string projectId)
    {
        Db = new FirestoreDbBuilder
        {
            ProjectId = projectId,
            EmulatorDetection = EmulatorDetection.EmulatorOrProduction,
        }.Build();
    }

    public FirestoreDb Db { get; }
}
