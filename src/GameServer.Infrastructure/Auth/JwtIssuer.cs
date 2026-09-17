using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameServer.Application.Sessions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GameServer.Infrastructure.Auth;

public sealed class JwtIssuer : IJwtIssuer
{
    private readonly JwtOptions _opts;
    private readonly SigningCredentials _credentials;

    public JwtIssuer(IOptions<JwtOptions> opts)
    {
        _opts = opts.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opts.SigningKey));
        _credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    public string IssueAccessToken(JwtClaims claims)
    {
        var token = new JwtSecurityToken(
            issuer: _opts.Issuer,
            audience: _opts.Audience,
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, claims.Sub),
                new Claim("game_id", claims.GameId),
                new Claim("role", claims.Role)
            },
            expires: DateTime.UtcNow.AddMinutes(_opts.AccessTokenMinutes),
            signingCredentials: _credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
