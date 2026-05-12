using System.Collections.Generic;
using System.Linq;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public static class ArtResolver
{
    public static List<ArtDefinition> GetUnlockedArts(Dictionary<Element, int> totals)
    {
        return ArtDatabase.All
            .Where(art => MeetsRequirements(art, totals))
            .ToList();
    }

    public static bool MeetsRequirements(ArtDefinition art, Dictionary<Element, int> totals)
    {
        foreach (var req in art.Requirements)
        {
            if (!totals.TryGetValue(req.Key, out var value) || value < req.Value)
                return false;
        }

        return true;
    }
}