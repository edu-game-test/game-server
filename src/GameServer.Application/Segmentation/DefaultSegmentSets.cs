using MetaFramework.Segmentation;

namespace GameServer.Application.Segmentation;

public static class DefaultSegmentSets
{
    public static SegmentSet Match3() => new SegmentSet
    {
        Version = 1,
        PublishedAt = DateTime.UtcNow,
        PublishedBy = "system",
        Segments = new List<SegmentDefinition>
        {
            new SegmentDefinition
            {
                SegmentId = "new_player",
                DisplayName = "New Player",
                Description = "Player has completed 5 or fewer levels",
                Predicates = new List<Predicate> { new Predicate { Field = SegmentFields.LevelNumber, Op = "lte", Value = 5L } }
            },
            new SegmentDefinition
            {
                SegmentId = "spender",
                DisplayName = "Spender",
                Description = "Player has made at least one purchase",
                Predicates = new List<Predicate> { new Predicate { Field = SegmentFields.HasPurchased, Op = "eq", Value = true } }
            },
            new SegmentDefinition
            {
                SegmentId = "highly_engaged",
                DisplayName = "Highly Engaged",
                Description = "Player active 5+ of the last 7 days",
                Predicates = new List<Predicate> { new Predicate { Field = SegmentFields.SessionCount7d, Op = "gte", Value = 5L } }
            },
            new SegmentDefinition
            {
                SegmentId = "veteran",
                DisplayName = "Veteran",
                Description = "Player has been installed for 30+ days",
                Predicates = new List<Predicate> { new Predicate { Field = SegmentFields.DaysSinceInstall, Op = "gte", Value = 30L } }
            }
        },
        SplitTests = new List<SplitTest>()
    };
}
