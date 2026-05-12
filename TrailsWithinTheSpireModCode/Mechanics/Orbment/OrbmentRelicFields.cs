using BaseLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Relics;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public static class OrbmentRelicFields
{
    public static readonly string EmptySlotsEncoded =
        string.Join("|", Enumerable.Repeat("", BattleOrbmentState.MaxSlots));

    public static readonly SavedSpireField<BattleOrbment, int> UnlockedSlots =
        new(() => 1, "TWTS_ORBMENT_UNLOCKED_SLOTS");

    public static readonly SavedSpireField<BattleOrbment, string> OwnedQuartz =
        new(() => "", "TWTS_ORBMENT_OWNED_QUARTZ");

    public static readonly SavedSpireField<BattleOrbment, string> EquippedQuartz =
        new(() => EmptySlotsEncoded, "TWTS_ORBMENT_EQUIPPED_QUARTZ");

    public static readonly SavedSpireField<BattleOrbment, int> AppliedMaxHpBonus =
        new(() => 0, "TWTS_ORBMENT_APPLIED_MAX_HP_BONUS");

    public static void Normalize(BattleOrbment relic)
    {
        var unlockedSlots = UnlockedSlots[relic];

        if (unlockedSlots < 1)
            unlockedSlots = 1;

        if (unlockedSlots > BattleOrbmentState.MaxSlots)
            unlockedSlots = BattleOrbmentState.MaxSlots;

        UnlockedSlots[relic] = unlockedSlots;

        EquippedQuartz[relic] = EncodeSlots(DecodeSlots(EquippedQuartz[relic]));
        OwnedQuartz[relic] = EncodeOwnedQuartz(DecodeOwnedQuartz(OwnedQuartz[relic]));

        if (AppliedMaxHpBonus[relic] < 0)
            AppliedMaxHpBonus[relic] = 0;
    }

    public static List<string> DecodeOwnedQuartz(string? encoded)
    {
        if (string.IsNullOrWhiteSpace(encoded))
            return new List<string>();

        return encoded
            .Split('|', StringSplitOptions.None)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToList();
    }

    public static string EncodeOwnedQuartz(IEnumerable<string> quartzIds)
    {
        return string.Join("|", quartzIds.Where(id => !string.IsNullOrWhiteSpace(id)));
    }

    public static List<string?> DecodeSlots(string? encoded)
    {
        var slots = new List<string?>();

        if (!string.IsNullOrWhiteSpace(encoded))
        {
            slots.AddRange(
                encoded
                    .Split('|', StringSplitOptions.None)
                    .Select(id => string.IsNullOrWhiteSpace(id) ? null : id)
            );
        }

        while (slots.Count < BattleOrbmentState.MaxSlots)
            slots.Add(null);

        while (slots.Count > BattleOrbmentState.MaxSlots)
            slots.RemoveAt(slots.Count - 1);

        return slots;
    }

    public static string EncodeSlots(IEnumerable<string?> slotIds)
    {
        var normalized = slotIds
            .Take(BattleOrbmentState.MaxSlots)
            .Select(id => string.IsNullOrWhiteSpace(id) ? "" : id)
            .ToList();

        while (normalized.Count < BattleOrbmentState.MaxSlots)
            normalized.Add("");

        return string.Join("|", normalized);
    }
}