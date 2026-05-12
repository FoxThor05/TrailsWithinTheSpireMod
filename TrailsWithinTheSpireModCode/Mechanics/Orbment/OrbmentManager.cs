using Godot;
using MegaCrit.Sts2.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Relics;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public static class OrbmentManager
{
    private static readonly BattleOrbmentState FallbackState = new();
    private static readonly List<string> FallbackOwnedQuartzIds = new();

    private static BattleOrbment? _activeBattleOrbment;

    public static BattleOrbmentState Current =>
        _activeBattleOrbment != null
            ? new BattleOrbmentState(_activeBattleOrbment)
            : FallbackState;

    public static IReadOnlyList<string> OwnedQuartzIds =>
        _activeBattleOrbment != null
            ? GetOwnedQuartzIdsFromRelic(_activeBattleOrbment)
            : FallbackOwnedQuartzIds;

    public static event Action? OrbmentChanged;

    public static void RegisterBattleOrbment(BattleOrbment battleOrbment)
    {
        _activeBattleOrbment = battleOrbment;
        OrbmentRelicFields.Normalize(battleOrbment);

        GD.Print("ORBMENT_LOG: BattleOrbment registered as active Orbment save holder.");
    }

    public static void Reset()
    {
        if (_activeBattleOrbment != null)
        {
            OrbmentRelicFields.UnlockedSlots[_activeBattleOrbment] = 1;
            OrbmentRelicFields.OwnedQuartz[_activeBattleOrbment] = "";
            OrbmentRelicFields.EquippedQuartz[_activeBattleOrbment] = OrbmentRelicFields.EmptySlotsEncoded;
            OrbmentRelicFields.Normalize(_activeBattleOrbment);
        }
        else
        {
            FallbackOwnedQuartzIds.Clear();
        }

        NotifyOrbmentChanged();
    }

    public static void AddQuartz(string quartzId)
    {
        if (QuartzDatabase.GetById(quartzId) == null)
        {
            GD.PrintErr($"ORBMENT_LOG: Cannot add unknown quartz '{quartzId}'.");
            return;
        }

        if (_activeBattleOrbment != null)
        {
            var ownedQuartz = GetOwnedQuartzIdsFromRelic(_activeBattleOrbment);
            ownedQuartz.Add(quartzId);
            SetOwnedQuartzIdsOnRelic(_activeBattleOrbment, ownedQuartz);

            GD.Print($"ORBMENT_LOG: Added quartz '{quartzId}'. Owned quartz count: {ownedQuartz.Count}");
        }
        else
        {
            FallbackOwnedQuartzIds.Add(quartzId);

            GD.Print($"ORBMENT_LOG: Added quartz '{quartzId}' to fallback inventory. Owned quartz count: {FallbackOwnedQuartzIds.Count}");
        }

        NotifyOrbmentChanged();
    }

    public static int CountOwnedQuartz(string quartzId)
    {
        return OwnedQuartzIds.Count(id => id == quartzId);
    }

    public static IReadOnlyList<QuartzDefinition> GetEquippedQuartz()
    {
        var equippedQuartz = new List<QuartzDefinition>();

        for (var i = 0; i < BattleOrbmentState.MaxSlots; i++)
        {
            var quartz = Current.GetSlotQuartz(i);

            if (quartz != null)
                equippedQuartz.Add(quartz);
        }

        return equippedQuartz;
    }

    public static bool EquipOwnedQuartzToSlot(string quartzId, int slotIndex)
    {
        var quartz = QuartzDatabase.GetById(quartzId);

        if (quartz == null)
        {
            GD.PrintErr($"ORBMENT_LOG: Cannot equip unknown quartz '{quartzId}'.");
            return false;
        }

        if (!Current.IsSlotUnlocked(slotIndex))
        {
            GD.PrintErr($"ORBMENT_LOG: Cannot equip '{quartzId}' to locked/invalid slot {slotIndex}.");
            return false;
        }

        if (!RemoveOwnedQuartzNoNotify(quartzId))
        {
            GD.PrintErr($"ORBMENT_LOG: Cannot equip '{quartzId}' because it is not in inventory.");
            return false;
        }

        var oldQuartz = Current.GetSlotQuartz(slotIndex);

        if (oldQuartz != null)
            AddOwnedQuartzNoNotify(oldQuartz.Id);

        Current.EquipQuartz(slotIndex, quartz);

        GD.Print($"ORBMENT_LOG: Equipped inventory quartz '{quartzId}' to slot {slotIndex}.");

        NotifyOrbmentChanged();

        return true;
    }

    public static bool MoveSlotQuartzToSlot(int sourceSlotIndex, int targetSlotIndex)
    {
        if (sourceSlotIndex == targetSlotIndex)
            return false;

        if (!Current.IsSlotUnlocked(sourceSlotIndex) || !Current.IsSlotUnlocked(targetSlotIndex))
        {
            GD.PrintErr($"ORBMENT_LOG: Cannot move quartz from slot {sourceSlotIndex} to slot {targetSlotIndex}; one slot is locked/invalid.");
            return false;
        }

        var sourceQuartz = Current.GetSlotQuartz(sourceSlotIndex);

        if (sourceQuartz == null)
        {
            GD.PrintErr($"ORBMENT_LOG: Source slot {sourceSlotIndex} has no quartz.");
            return false;
        }

        var targetQuartz = Current.GetSlotQuartz(targetSlotIndex);

        Current.SetSlotQuartz(targetSlotIndex, sourceQuartz);
        Current.SetSlotQuartz(sourceSlotIndex, targetQuartz);

        GD.Print($"ORBMENT_LOG: Moved quartz from slot {sourceSlotIndex} to slot {targetSlotIndex}.");

        NotifyOrbmentChanged();

        return true;
    }

    public static bool UnequipSlotToInventory(int slotIndex)
    {
        if (!Current.IsSlotUnlocked(slotIndex))
        {
            GD.PrintErr($"ORBMENT_LOG: Cannot unequip locked/invalid slot {slotIndex}.");
            return false;
        }

        var removedQuartz = Current.ClearSlot(slotIndex);

        if (removedQuartz == null)
        {
            GD.PrintErr($"ORBMENT_LOG: Slot {slotIndex} has no quartz to unequip.");
            return false;
        }

        AddOwnedQuartzNoNotify(removedQuartz.Id);

        GD.Print($"ORBMENT_LOG: Unequipped '{removedQuartz.Id}' from slot {slotIndex} to inventory.");

        NotifyOrbmentChanged();

        return true;
    }

    public static void NotifyOrbmentChanged()
    {
        OrbmentChanged?.Invoke();

        TaskHelper.RunSafely(QuartzEffectDispatcher.ApplyPassiveEffectsFromActiveBattleOrbment());
    }

    private static List<string> GetOwnedQuartzIdsFromRelic(BattleOrbment relic)
    {
        OrbmentRelicFields.Normalize(relic);
        return OrbmentRelicFields.DecodeOwnedQuartz(OrbmentRelicFields.OwnedQuartz[relic]);
    }

    private static void SetOwnedQuartzIdsOnRelic(BattleOrbment relic, List<string> ownedQuartzIds)
    {
        OrbmentRelicFields.OwnedQuartz[relic] = OrbmentRelicFields.EncodeOwnedQuartz(ownedQuartzIds);
        OrbmentRelicFields.Normalize(relic);
    }

    private static void AddOwnedQuartzNoNotify(string quartzId)
    {
        if (_activeBattleOrbment != null)
        {
            var ownedQuartz = GetOwnedQuartzIdsFromRelic(_activeBattleOrbment);
            ownedQuartz.Add(quartzId);
            SetOwnedQuartzIdsOnRelic(_activeBattleOrbment, ownedQuartz);
        }
        else
        {
            FallbackOwnedQuartzIds.Add(quartzId);
        }
    }

    private static bool RemoveOwnedQuartzNoNotify(string quartzId)
    {
        if (_activeBattleOrbment != null)
        {
            var ownedQuartz = GetOwnedQuartzIdsFromRelic(_activeBattleOrbment);
            var index = ownedQuartz.IndexOf(quartzId);

            if (index < 0)
                return false;

            ownedQuartz.RemoveAt(index);
            SetOwnedQuartzIdsOnRelic(_activeBattleOrbment, ownedQuartz);

            return true;
        }

        var fallbackIndex = FallbackOwnedQuartzIds.IndexOf(quartzId);

        if (fallbackIndex < 0)
            return false;

        FallbackOwnedQuartzIds.RemoveAt(fallbackIndex);

        return true;
    }
}