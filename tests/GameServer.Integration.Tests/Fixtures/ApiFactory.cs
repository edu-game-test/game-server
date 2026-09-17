using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace GameServer.Integration.Tests.Fixtures;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Firestore:ProjectId"] = FirestoreEmulatorFixture.ProjectId,
                ["Firebase:ProjectId"] = FirestoreEmulatorFixture.ProjectId,
                ["Auth:AllowFakeTokens"] = "true",
                ["Jwt:SigningKey"] = "integration-test-signing-key-0123456789abcdef",
                ["Jwt:Issuer"] = "game-server",
                ["Jwt:Audience"] = "game-client",
                ["Jwt:AccessTokenMinutes"] = "60",
                ["Jwt:RefreshTokenDays"] = "30",
                ["Games:Allowed:0"] = "match3",
                ["Admin:ApiKey"] = "test-admin-key",
            });
        });
    }
}
