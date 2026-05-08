namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public static class OrbmentManager
{
    public static BattleOrbmentState Current { get; private set; } = new();

    public static void Reset()
    {
        Current = new BattleOrbmentState();
    }
    public static List<string> OwnedQuartzIds { get; } = new();

    public static void AddQuartz(string quartzId)
    {
        OwnedQuartzIds.Add(quartzId);
    }
}