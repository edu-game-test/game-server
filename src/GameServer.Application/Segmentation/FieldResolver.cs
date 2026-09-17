using GameServer.Domain.Documents;
using MetaFramework.Segmentation;

namespace GameServer.Application.Segmentation;

public static class FieldResolver
{
    public static Func<string, object?> ForPlayer(PlayerDocument player)
        => field => field switch
        {
            SegmentFields.LevelNumber => (object?)player.MaxLevelReached,
            SegmentFields.StarsTotal => player.StarsTotal,
            SegmentFields.DaysSinceInstall => DaysSince(player.InstalledAt.ToDateTime()),
            SegmentFields.SessionCount7d => player.RecentSessionDays.Count,
            SegmentFields.LifetimeSpendUsd => player.LifetimeSpendUsd,
            SegmentFields.Platform => player.Platform,
            SegmentFields.CountryCode => player.CountryCode,
            SegmentFields.HasPurchased => player.LifetimeSpendUsd > 0,
            SegmentFields.LastSessionDaysAgo => DaysSince(player.LastLoginAt.ToDateTime()),
            _ => null
        };

    private static int DaysSince(DateTime dt) => (int)(DateTime.UtcNow - dt.Date).TotalDays;
}
