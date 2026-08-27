namespace GameServer.Domain.Interfaces;

public interface IPlayerRepository
{
    Task<PlayerEntity?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<PlayerEntity?> GetByFirebaseUidAsync(string uid, CancellationToken ct = default);
    Task<PlayerEntity> CreateAsync(PlayerEntity player, CancellationToken ct = default);
    Task UpdateAsync(PlayerEntity player, CancellationToken ct = default);
}
