using Godot;
using System.Collections.Generic;
using System.Linq;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Relics;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public class BattleOrbmentState
{
    public const int MaxSlots = 7;

    private readonly BattleOrbment? _relic;

    private int _fallbackUnlockedSlots = 1;
    private readonly QuartzDefinition?[] _fallbackSlots = new QuartzDefinition?[MaxSlots];

    public BattleOrbmentState()
    {
    }

    public BattleOrbmentState(BattleOrbment relic)
    {
        _relic = relic;
        OrbmentRelicFields.Normalize(relic);
    }

    public int UnlockedSlots
    {
        get
        {
            if (_relic == null)
                return _fallbackUnlockedSlots;

            OrbmentRelicFields.Normalize(_relic);
            return OrbmentRelicFields.UnlockedSlots[_relic];
        }
    }

    public void UnlockSlot()
    {
        if (_relic != null)
        {
            OrbmentRelicFields.Normalize(_relic);

            if (OrbmentRelicFields.UnlockedSlots[_relic] < MaxSlots)
                OrbmentRelicFields.UnlockedSlots[_relic]++;

            return;
        }

        if (_fallbackUnlockedSlots < MaxSlots)
            _fallbackUnlockedSlots++;
    }

    public bool IsSlotUnlocked(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < UnlockedSlots && slotIndex < MaxSlots;
    }

    public bool EquipQuartz(int slotIndex, QuartzDefinition quartz)
    {
        return SetSlotQuartz(slotIndex, quartz);
    }

    public QuartzDefinition? GetSlotQuartz(int slotIndex)
    {
        var quartzId = GetSlotQuartzId(slotIndex);

        if (string.IsNullOrWhiteSpace(quartzId))
            return null;

        return QuartzDatabase.GetById(quartzId);
    }

    public string? GetSlotQuartzId(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MaxSlots)
            return null;

        if (_relic != null)
        {
            OrbmentRelicFields.Normalize(_relic);

            var slots = OrbmentRelicFields.DecodeSlots(OrbmentRelicFields.EquippedQuartz[_relic]);

            return string.IsNullOrWhiteSpace(slots[slotIndex])
                ? null
                : slots[slotIndex];
        }

        return _fallbackSlots[slotIndex]?.Id;
    }

    public QuartzDefinition? ClearSlot(int slotIndex)
    {
        if (!IsSlotUnlocked(slotIndex))
            return null;

        var oldQuartz = GetSlotQuartz(slotIndex);

        SetSlotQuartz(slotIndex, null);

        return oldQuartz;
    }

    public bool SetSlotQuartz(int slotIndex, QuartzDefinition? quartz)
    {
        if (!IsSlotUnlocked(slotIndex))
            return false;

        if (_relic != null)
        {
            OrbmentRelicFields.Normalize(_relic);

            var slots = OrbmentRelicFields.DecodeSlots(OrbmentRelicFields.EquippedQuartz[_relic]);
            slots[slotIndex] = quartz?.Id;

            OrbmentRelicFields.EquippedQuartz[_relic] = OrbmentRelicFields.EncodeSlots(slots);

            return true;
        }

        _fallbackSlots[slotIndex] = quartz;
        return true;
    }

    public List<string?> GetSlots()
    {
        var result = new List<string?>();

        for (var i = 0; i < UnlockedSlots; i++)
            result.Add(GetSlotQuartzId(i));

        return result;
    }

    public void SetSlot(int slotIndex, string? quartzId)
    {
        if (!IsSlotUnlocked(slotIndex))
            return;

        if (string.IsNullOrWhiteSpace(quartzId))
        {
            SetSlotQuartz(slotIndex, null);
            return;
        }

        var quartz = QuartzDatabase.GetById(quartzId);

        if (quartz != null)
        {
            SetSlotQuartz(slotIndex, quartz);
        }
        else
        {
            GD.PrintErr($"Attempted to slot unknown quartz ID: {quartzId}");
        }
    }

    public Dictionary<Element, int> GetElementTotals()
    {
        var totals = new Dictionary<Element, int>();

        for (var i = 0; i < MaxSlots; i++)
        {
            if (!IsSlotUnlocked(i))
                continue;

            var quartz = GetSlotQuartz(i);

            if (quartz == null)
                continue;

            foreach (var pair in quartz.ElementValues)
            {
                if (!totals.ContainsKey(pair.Key))
                    totals[pair.Key] = 0;

                totals[pair.Key] += pair.Value;
            }
        }

        return totals;
    }
}