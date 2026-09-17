using GameServer.Application.Admin;
using GameServer.Application.FeatureFlags;
using GameServer.Application.Segmentation;
using GameServer.Application.Segments;
using GameServer.Application.Sessions;
using GameServer.Domain.Documents;
using GameServer.Infrastructure.Firestore;
using Google.Cloud.Firestore;
using MetaFramework.FeatureFlags;
using MetaFramework.Segmentation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GameServer.API.Controllers;

[ApiController]
[Route("api/v1/admin")]
public class AdminController : ControllerBase
{
    private readonly IJwtIssuer _jwt;
    private readonly IFlagService _flags;
    private readonly ISegmentService _segments;
    private readonly IFlagSetRepository _flagRepo;
    private readonly ISegmentSetRepository _segmentRepo;
    private readonly AuditRepository _audit;
    private readonly PlayerFlagOverridesRepository _overrides;
    private readonly AdminOptions _adminOpts;

    public AdminController(
        IJwtIssuer jwt,
        IFlagService flags,
        ISegmentService segments,
        IFlagSetRepository flagRepo,
        ISegmentSetRepository segmentRepo,
        AuditRepository audit,
        PlayerFlagOverridesRepository overrides,
        IOptions<AdminOptions> adminOpts)
    {
        _jwt = jwt;
        _flags = flags;
        _segments = segments;
        _flagRepo = flagRepo;
        _segmentRepo = segmentRepo;
        _audit = audit;
        _overrides = overrides;
        _adminOpts = adminOpts.Value;
    }

    [AllowAnonymous]
    [HttpPost("auth")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Auth([FromBody] AdminAuthRequest request)
    {
        if (string.IsNullOrEmpty(_adminOpts.ApiKey) || request.ApiKey != _adminOpts.ApiKey)
            return Unauthorized(new { error = "invalid_api_key" });

        var token = _jwt.IssueAccessToken(new JwtClaims { Sub = "admin", GameId = "admin", Role = "admin" });
        return Ok(new { accessToken = token });
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{gameId}/flags")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PutFlagSet(string gameId, [FromBody] FlagSet flagSet, CancellationToken ct)
    {
        var errors = FlagSetValidator.Validate(flagSet);
        if (errors.Count > 0)
            return UnprocessableEntity(new { errors });

        await _flags.SaveFlagSetAsync(gameId, flagSet, ct);
        await WriteAudit(gameId, "flags.put", flagSet.Version.ToString(), ct);
        return Ok(new { version = flagSet.Version });
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{gameId}/segments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PutSegmentSet(string gameId, [FromBody] SegmentSet segmentSet, CancellationToken ct)
    {
        var errors = SegmentSetValidator.Validate(segmentSet);
        if (errors.Count > 0)
            return UnprocessableEntity(new { errors });

        await _segmentRepo.SaveAsync(gameId, segmentSet, ct);
        _segments.Invalidate(gameId);
        await WriteAudit(gameId, "segments.put", segmentSet.Version.ToString(), ct);
        return Ok(new { version = segmentSet.Version });
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("{gameId}/players/{playerId}/flag-overrides")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFlagOverrides(string gameId, string playerId, CancellationToken ct)
    {
        var doc = await _overrides.GetAsync(gameId, playerId, ct);
        return Ok(new { overrides = doc?.Overrides ?? new Dictionary<string, object?>() });
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{gameId}/players/{playerId}/flag-overrides")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PutFlagOverrides(string gameId, string playerId, [FromBody] Dictionary<string, object?> overrides, CancellationToken ct)
    {
        var doc = new PlayerFlagOverridesDocument
        {
            PlayerId = playerId,
            Overrides = overrides,
            UpdatedAt = Timestamp.GetCurrentTimestamp()
        };
        await _overrides.SaveAsync(gameId, playerId, doc, ct);
        await WriteAudit(gameId, "player.flag_overrides.put", $"player={playerId} count={overrides.Count}", ct);
        return Ok(new { count = overrides.Count });
    }

    private Task WriteAudit(string gameId, string action, string details, CancellationToken ct)
        => _audit.AppendAsync(gameId, new AuditDocument
        {
            Id = Guid.NewGuid().ToString("N"),
            Action = action,
            Actor = User.FindFirst("sub")?.Value ?? "admin",
            Details = details,
            OccurredAt = Timestamp.GetCurrentTimestamp()
        }, ct);
}

public sealed record AdminAuthRequest(string ApiKey);
