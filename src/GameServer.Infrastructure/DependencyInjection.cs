using GameServer.Domain.Repositories;
using GameServer.Infrastructure.Firestore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameServer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var projectId = configuration["Firestore:ProjectId"];
        if (string.IsNullOrWhiteSpace(projectId))
            throw new InvalidOperationException("Firestore:ProjectId is not configured.");

        services.AddSingleton(new FirestoreContext(projectId));
        services.AddSingleton<IPlayerRepository, PlayerRepository>();
        services.AddSingleton<IAccountRepository, AccountRepository>();
        return services;
    }
}
