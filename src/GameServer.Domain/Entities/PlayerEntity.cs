namespace GameServer.Domain;

public class PlayerEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FirebaseUid { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public long ExperiencePoints { get; set; }
    public int Gold { get; set; }
    public int Gems { get; set; }
    public int SummonShards { get; set; }
    public int EnergyPoints { get; set; } = 100;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;
}
