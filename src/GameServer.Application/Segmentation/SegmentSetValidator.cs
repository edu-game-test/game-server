using MetaFramework.Segmentation;

namespace GameServer.Application.Segmentation;

public static class SegmentSetValidator
{
    public static IReadOnlyList<string> Validate(SegmentSet segmentSet)
    {
        var errors = new List<string>();
        if (segmentSet.Segments == null)
        {
            errors.Add("Segments collection is required.");
            return errors;
        }
        var ids = segmentSet.Segments.Select(s => s.SegmentId).ToList();
        if (ids.Distinct().Count() != ids.Count)
            errors.Add("Segment IDs must be unique.");
        foreach (var def in segmentSet.Segments)
        {
            if (string.IsNullOrWhiteSpace(def.SegmentId))
                errors.Add("A segment is missing its SegmentId.");
        }
        return errors;
    }
}
