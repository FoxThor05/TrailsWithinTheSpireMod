using System.Collections.Generic;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public static class ArtDatabase
{
    public static readonly ArtDefinition Crest = new ArtDefinition
    {
        Id = "crest",
        EnergyCost = 1,
        Requirements = new Dictionary<Element, int>
        {
            { Element.Earth, 1 },
            { Element.Mirage, 1 }
        }
    };

    public static readonly ArtDefinition Tear = new ArtDefinition
    {
        Id = "tear",
        EnergyCost = 1,
        IsHealing = true,
        Requirements = new Dictionary<Element, int>
        {
            { Element.Water, 1 }
        }
    };

    public static readonly ArtDefinition FireBolt = new ArtDefinition
    {
        Id = "fire_bolt",
        EnergyCost = 1,
        Requirements = new Dictionary<Element, int>
        {
            { Element.Fire, 1 }
        }
    };

    public static List<ArtDefinition> All => new()
    {
        Crest,
        Tear,
        FireBolt
    };
}