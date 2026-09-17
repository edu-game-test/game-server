using MetaFramework.FeatureFlags;

namespace GameServer.Application.FeatureFlags;

public static class FlagSetValidator
{
    public static IReadOnlyList<string> Validate(FlagSet flagSet)
    {
        var errors = new List<string>();
        if (flagSet.Flags == null || flagSet.Flags.Count == 0)
        {
            errors.Add("Flags collection must not be empty.");
            return errors;
        }
        foreach (var def in flagSet.Flags)
        {
            if (string.IsNullOrWhiteSpace(def.Key))
                errors.Add("A flag is missing its Key.");
            if (def.DefaultValue == null)
                errors.Add($"Flag '{def.Key}' has null DefaultValue.");
            if (def.Rollout is { Percentage: < 0 or > 100 })
                errors.Add($"Flag '{def.Key}' rollout percentage {def.Rollout.Percentage} is out of range [0,100].");
        }
        return errors;
    }
}
