using System;
using System.Collections.Generic;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public static class OrbmentCombatState
{
    public static HashSet<string> UsedHealingArts { get; } = new();

    public static int MaxCastsThisTurn { get; private set; } = 1;

    public static int UsedCastsThisTurn { get; private set; }

    public static int RemainingCastsThisTurn => Math.Max(0, MaxCastsThisTurn - UsedCastsThisTurn);

    public static bool UsedArtThisTurn => RemainingCastsThisTurn <= 0;

    public static event Action? StateChanged;

    public static void SetMaxCastsThisTurn(int amount)
    {
        MaxCastsThisTurn = Math.Max(0, amount);
        UsedCastsThisTurn = Math.Min(UsedCastsThisTurn, MaxCastsThisTurn);
        StateChanged?.Invoke();
    }

    public static void MarkArtUsedThisTurn()
    {
        UseCast();
    }

    public static void UseCast()
    {
        if (RemainingCastsThisTurn <= 0)
            return;

        UsedCastsThisTurn++;
        StateChanged?.Invoke();
    }

    public static void ResetTurn()
    {
        MaxCastsThisTurn = 1;
        UsedCastsThisTurn = 0;
        StateChanged?.Invoke();
    }

    public static void ResetTurn(int maxCastsThisTurn)
    {
        MaxCastsThisTurn = Math.Max(0, maxCastsThisTurn);
        UsedCastsThisTurn = 0;
        StateChanged?.Invoke();
    }

    public static void ResetCombat()
    {
        UsedHealingArts.Clear();
        ResetTurn();
        StateChanged?.Invoke();
    }
}