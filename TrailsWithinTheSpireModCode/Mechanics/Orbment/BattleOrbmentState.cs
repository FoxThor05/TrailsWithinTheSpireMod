using System.Collections.Generic;
using System.Linq;
using Godot;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public class BattleOrbmentState
{
    public const int MaxSlots = 7;

    public int UnlockedSlots { get; private set; } = 1;

    private readonly QuartzDefinition?[] _slots = new QuartzDefinition?[MaxSlots];

    public void UnlockSlot()
    {
        if (UnlockedSlots < MaxSlots)
            UnlockedSlots++;
    }

    public bool EquipQuartz(int slotIndex, QuartzDefinition quartz)
    {
        if (slotIndex < 0 || slotIndex >= MaxSlots)
            return false;

        if (slotIndex >= UnlockedSlots)
            return false;

        _slots[slotIndex] = quartz;
        return true;
    }

    public List<string?> GetSlots()
    {
        return _slots.Take(UnlockedSlots).Select(q => q?.Id).ToList();
    }

    public string? GetSlotQuartzId(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MaxSlots)
            return null;

        return _slots[slotIndex]?.Id;
    }

    public void SetSlot(int slotIndex, string? quartzId)
    {
        if (slotIndex < 0 || slotIndex >= MaxSlots)
            return;

        if (slotIndex >= UnlockedSlots)
            return;

        if (quartzId == null)
        {
            _slots[slotIndex] = null;
            return;
        }

        QuartzDefinition? quartz = QuartzDatabase.All.Find(q => q.Id == quartzId);

        if (quartz != null)
        {
            _slots[slotIndex] = quartz;
        }
        else
        {
            GD.PrintErr($"Attempted to slot unknown quartz ID: {quartzId}");
        }
    }

    public Dictionary<Element, int> GetElementTotals()
    {
        var totals = new Dictionary<Element, int>();

        foreach (var quartz in _slots.Where(q => q != null))
        {
            foreach (var pair in quartz!.ElementValues)
            {
                if (!totals.ContainsKey(pair.Key))
                    totals[pair.Key] = 0;

                totals[pair.Key] += pair.Value;
            }
        }

        return totals;
    }
}