using System.Collections.Generic;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public static class OrbmentCombatState
{
    public static HashSet<string> UsedHealingArts { get; } = new();
    public static bool UsedArtThisTurn { get; private set; }

    public static void MarkArtUsedThisTurn()
    {
        UsedArtThisTurn = true;
    }

    public static void ResetTurn()
    {
        UsedArtThisTurn = false;
    }
    public static void ResetCombat()
    {
        UsedHealingArts.Clear();
    }
}