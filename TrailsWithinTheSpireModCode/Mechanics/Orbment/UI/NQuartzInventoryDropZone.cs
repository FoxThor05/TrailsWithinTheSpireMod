#nullable enable
using Godot;
using System;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

public partial class NQuartzInventoryDropZone : NClickableControl
{
    public event Action<string, string, int>? QuartzDroppedOnInventory;

    public override void _Ready()
    {
        base._Ready();

        MouseFilter = MouseFilterEnum.Stop;
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        if (!NQuartzDisplay.TryReadQuartzDragData(
                data,
                out _,
                out var source,
                out _))
        {
            return false;
        }

        return source == NQuartzDisplay.DragSourceSlot;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        if (!NQuartzDisplay.TryReadQuartzDragData(
                data,
                out var quartzId,
                out var source,
                out var sourceSlotIndex))
        {
            return;
        }

        if (source != NQuartzDisplay.DragSourceSlot)
            return;

        GD.Print($"NQuartzInventoryDropZone: Dropped quartz={quartzId}, source={source}, sourceSlot={sourceSlotIndex}");

        QuartzDroppedOnInventory?.Invoke(quartzId, source, sourceSlotIndex);
    }
}