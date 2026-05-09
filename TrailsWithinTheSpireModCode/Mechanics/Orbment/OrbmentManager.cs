using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public static class OrbmentManager
{
    public static BattleOrbmentState Current { get; private set; } = new();

    public static event Action? OwnedQuartzChanged;

    private static readonly List<string> _ownedQuartzIds = new();

    public static IReadOnlyList<string> OwnedQuartzIds => _ownedQuartzIds;

    public static void Reset()
    {
        Current = new BattleOrbmentState();
        _ownedQuartzIds.Clear();
        OwnedQuartzChanged?.Invoke();
    }

    public static void AddQuartz(string quartzId)
    {
        _ownedQuartzIds.Add(quartzId);

        GD.Print($"ORBMENT_LOG: Added quartz '{quartzId}'. Owned quartz count: {_ownedQuartzIds.Count}");

        OwnedQuartzChanged?.Invoke();
    }

    public static int CountOwnedQuartz(string quartzId)
    {
        return _ownedQuartzIds.Count(id => id == quartzId);
    }
}