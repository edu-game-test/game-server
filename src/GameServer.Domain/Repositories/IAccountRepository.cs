using GameServer.Domain.Documents;

namespace GameServer.Domain.Repositories;

public interface IAccountRepository
{
    Task<AccountDocument?> GetAsync(string firebaseUid, CancellationToken ct = default);
    Task UpsertAsync(AccountDocument account, CancellationToken ct = default);
}
