using FirebaseAdmin;
using GameServer.Application.Sessions;
using GameServer.Domain.Repositories;
using GameServer.Infrastructure.Auth;
using GameServer.Infrastructure.Firestore;
using Google.Apis.Auth.OAuth2;
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
        services.AddSingleton<IRefreshTokenRepository, RefreshTokenRepository>();

        services.Configure<JwtOptions>(opts =>
        {
            opts.Issuer = configuration["Jwt:Issuer"] ?? "";
            opts.Audience = configuration["Jwt:Audience"] ?? "";
            opts.SigningKey = configuration["Jwt:SigningKey"] ?? "";
            opts.AccessTokenMinutes = int.TryParse(configuration["Jwt:AccessTokenMinutes"], out var am) ? am : 60;
            opts.RefreshTokenDays = int.TryParse(configuration["Jwt:RefreshTokenDays"], out var rd) ? rd : 30;
        });
        services.Configure<SessionOptions>(opts =>
        {
            opts.AccessTokenMinutes = int.TryParse(configuration["Jwt:AccessTokenMinutes"], out var am) ? am : 60;
            opts.RefreshTokenDays = int.TryParse(configuration["Jwt:RefreshTokenDays"], out var rd) ? rd : 30;
        });

        services.AddSingleton<IJwtIssuer, JwtIssuer>();

        var allowFake = configuration["Auth:AllowFakeTokens"]?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
        if (allowFake)
        {
            services.AddSingleton<IFirebaseTokenVerifier, FakeTokenVerifier>();
        }
        else
        {
            if (FirebaseApp.DefaultInstance == null)
                FirebaseApp.Create(new AppOptions { Credential = GoogleCredential.GetApplicationDefault() });
            services.AddSingleton<IFirebaseTokenVerifier, FirebaseTokenVerifier>();
        }

        return services;
    }
}
