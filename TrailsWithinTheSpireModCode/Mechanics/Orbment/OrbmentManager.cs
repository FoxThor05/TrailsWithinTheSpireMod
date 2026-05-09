using Godot;
using System.Collections.Generic;
using System.Linq;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public static class OrbmentManager
{
    public static BattleOrbmentState Current { get; private set; } = new();

    private static readonly List<string> _ownedQuartzIds = new();

    public static IReadOnlyList<string> OwnedQuartzIds => _ownedQuartzIds;

    public static void Reset()
    {
        Current = new BattleOrbmentState();
        _ownedQuartzIds.Clear();
    }

    public static void AddQuartz(string quartzId)
    {
        if (QuartzDatabase.GetById(quartzId) == null)
        {
            GD.PrintErr($"ORBMENT_LOG: Cannot add unknown quartz '{quartzId}'.");
            return;
        }

        _ownedQuartzIds.Add(quartzId);

        GD.Print($"ORBMENT_LOG: Added quartz '{quartzId}'. Owned quartz count: {_ownedQuartzIds.Count}");
    }

    public static int CountOwnedQuartz(string quartzId)
    {
        return _ownedQuartzIds.Count(id => id == quartzId);
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
            _ownedQuartzIds.Add(oldQuartz.Id);

        Current.EquipQuartz(slotIndex, quartz);

        GD.Print($"ORBMENT_LOG: Equipped inventory quartz '{quartzId}' to slot {slotIndex}.");

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

        _ownedQuartzIds.Add(removedQuartz.Id);

        GD.Print($"ORBMENT_LOG: Unequipped '{removedQuartz.Id}' from slot {slotIndex} to inventory.");

        return true;
    }

    private static bool RemoveOwnedQuartzNoNotify(string quartzId)
    {
        var index = _ownedQuartzIds.IndexOf(quartzId);

        if (index < 0)
            return false;

        _ownedQuartzIds.RemoveAt(index);
        return true;
    }
}