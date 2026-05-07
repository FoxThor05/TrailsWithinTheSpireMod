namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public static class OrbmentManager
{
    public static BattleOrbmentState Current { get; private set; } = new();

    public static void Reset()
    {
        Current = new BattleOrbmentState();
    }
}