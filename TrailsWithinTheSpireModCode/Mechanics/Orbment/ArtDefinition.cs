using System.Collections.Generic;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public class ArtDefinition
{
    public string Id { get; set; }

    public Dictionary<Element, int> Requirements { get; set; } = new();

    public int EnergyCost { get; set; }

    public bool IsHealing { get; set; } = false;
}