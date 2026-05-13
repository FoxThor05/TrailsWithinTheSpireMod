using System;
using System.Collections.Generic;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public static class OrbmentCombatState
{
    public const int BaseCastsPerTurn = 1;

    public static HashSet<string> UsedHealingArts { get; } = new();

    public static int MaxCastsThisTurn { get; private set; } = BaseCastsPerTurn;

    public static int UsedCastsThisTurn { get; private set; }

    public static int RemainingCastsThisTurn => Math.Max(0, MaxCastsThisTurn - UsedCastsThisTurn);

    public static bool UsedArtThisTurn => RemainingCastsThisTurn <= 0;

    public static event Action? StateChanged;

    public static int GetCurrentMaxCastsPerTurn()
    {
        return BaseCastsPerTurn + QuartzEffectDispatcher.GetAdditionalCastsPerTurn();
    }

    public static void RefreshMaxCastsForCurrentTurn()
    {
        var newMaxCasts = GetCurrentMaxCastsPerTurn();

        MaxCastsThisTurn = Math.Max(0, newMaxCasts);
        UsedCastsThisTurn = Math.Min(UsedCastsThisTurn, MaxCastsThisTurn);

        StateChanged?.Invoke();
    }

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
        MaxCastsThisTurn = GetCurrentMaxCastsPerTurn();
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