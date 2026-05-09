using System.Collections.Generic;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public class QuartzDefinition
{
    public string Id { get; set; } = "";

    public Dictionary<Element, int> ElementValues { get; set; } = new();

    public int Tier { get; set; }

    public string IconPath { get; set; } = "";
}