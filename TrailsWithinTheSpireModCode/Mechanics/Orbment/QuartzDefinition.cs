using System.Collections.Generic;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public class QuartzDefinition
{
    public string Id { get; init; } = "";

    public int RewardSaveId { get; init; }

    public Dictionary<Element, int> ElementValues { get; init; } = new();

    public int Tier { get; init; }

    public string IconPath { get; init; } = "";
}