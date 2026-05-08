using System.Collections.Generic;
using System.Linq;
using Godot; // Added for GD.PrintErr

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public class BattleOrbmentState
{
    public const int MaxSlots = 6;

    public int UnlockedSlots { get; private set; } = 1;

    private readonly QuartzDefinition?[] _slots = new QuartzDefinition?[MaxSlots];

    public void UnlockSlot()
    {
        if (UnlockedSlots < MaxSlots)
            UnlockedSlots++;
    }

    public bool EquipQuartz(int slotIndex, QuartzDefinition quartz)
    {
        if (slotIndex >= UnlockedSlots)
            return false;

        _slots[slotIndex] = quartz;
        return true;
    }

    public List<string?> GetSlots()
    {
        return _slots.Take(UnlockedSlots).Select(q => q?.Id).ToList();
    }

    public void SetSlot(int slotIndex, string? quartzId)
    {
        if (slotIndex >= UnlockedSlots)
            return;

        if (quartzId == null)
        {
            _slots[slotIndex] = null;
        }
        else
        {
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
