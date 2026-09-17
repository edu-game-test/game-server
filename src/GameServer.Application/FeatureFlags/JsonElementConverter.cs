using System.Text.Json;
using MetaFramework.FeatureFlags;

namespace GameServer.Application.FeatureFlags;

/// <summary>Unwraps JsonElement values in FlagDefinition fields that arise when deserializing from stored JSON.</summary>
public static class JsonElementConverter
{
    public static object? Unwrap(object? value) => value is JsonElement el ? UnwrapElement(el) : value;

    private static object? UnwrapElement(JsonElement el) => el.ValueKind switch
    {
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Number => el.TryGetInt64(out var l) ? (object)l : el.GetDouble(),
        JsonValueKind.String => el.GetString(),
        JsonValueKind.Null => null,
        _ => el.GetRawText()
    };

    public static FlagSet UnwrapAll(FlagSet flagSet)
    {
        foreach (var def in flagSet.Flags)
        {
            def.DefaultValue = Unwrap(def.DefaultValue);
            if (def.Rollout != null)
            {
                def.Rollout.EnabledValue = Unwrap(def.Rollout.EnabledValue);
                def.Rollout.DisabledValue = Unwrap(def.Rollout.DisabledValue);
            }
            foreach (var sv in def.SegmentValues)
                sv.Value = Unwrap(sv.Value);
            foreach (var key in def.PlatformOverrides.Keys.ToList())
                def.PlatformOverrides[key] = Unwrap(def.PlatformOverrides[key]);
        }
        return flagSet;
    }
}
