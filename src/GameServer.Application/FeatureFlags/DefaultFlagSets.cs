using MetaFramework.FeatureFlags;

namespace GameServer.Application.FeatureFlags;

public static class DefaultFlagSets
{
    public static FlagSet Match3() => new FlagSet
    {
        Version = 1,
        PublishedAt = DateTime.UtcNow,
        PublishedBy = "system",
        Flags = new List<FlagDefinition>
        {
            new FlagDefinition { Key = "new_player_tutorial", Type = FlagType.Boolean, Description = "Show tutorial flow to new players", Owner = "gameplay", DefaultValue = true },
            new FlagDefinition { Key = "lives_recharge_time_minutes", Type = FlagType.Integer, Description = "Minutes to recharge a single life", Owner = "economy", DefaultValue = 30L },
            new FlagDefinition { Key = "max_lives", Type = FlagType.Integer, Description = "Maximum lives a player can hold", Owner = "economy", DefaultValue = 5L },
            new FlagDefinition { Key = "daily_reward_enabled", Type = FlagType.Boolean, Description = "Enable daily login reward screen", Owner = "engagement", DefaultValue = true },
            new FlagDefinition { Key = "booster_shuffle_enabled", Type = FlagType.Boolean, Description = "Enable shuffle booster in level", Owner = "gameplay", DefaultValue = true },
            new FlagDefinition { Key = "hard_level_threshold", Type = FlagType.Float, Description = "Win-rate below this value marks a level as hard", Owner = "liveops", DefaultValue = 0.4 }
        }
    };
}
