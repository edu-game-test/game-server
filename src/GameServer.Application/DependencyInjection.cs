using GameServer.Application.Segments;
using GameServer.Application.Sessions;
using Microsoft.Extensions.DependencyInjection;

namespace GameServer.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ISessionService, SessionService>();
        services.AddSingleton<ISegmentService, NullSegmentService>();
        return services;
    }
}
