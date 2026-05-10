using System.Collections.Generic;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public static class ArtCardExtensions
{
    public static ArtDefinition? GetArtDefinition(this IArtCard artCard)
    {
        return ArtDatabase.GetById(artCard.ArtId);
    }

    public static IReadOnlyDictionary<Element, int> GetElementRequirements(this IArtCard artCard)
    {
        return artCard.GetArtDefinition()?.Requirements
               ?? new Dictionary<Element, int>();
    }
}