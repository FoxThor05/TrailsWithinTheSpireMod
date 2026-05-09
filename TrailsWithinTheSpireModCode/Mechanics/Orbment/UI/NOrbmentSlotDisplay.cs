#nullable enable
using Godot;
using System;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

public partial class NOrbmentSlotDisplay : NClickableControl
{
    private const float SlotQuartzBaseSize = 64f;
    private const float SlotQuartzScale = 1.65f;

    private int _slotIndex = -1;
    private bool _isUnlocked;

    private TextureButton? _slotButton;
    private Control? _quartzMount;
    private TextureRect? _lockOverlay;
    private TextureRect? _selectionGlow;

    public event Action<int, string, string, int>? QuartzDroppedOnSlot;

    public override void _Ready()
    {
        base._Ready();

        CustomMinimumSize = new Vector2(96, 96);
        MouseFilter = MouseFilterEnum.Stop;

        _slotButton = GetNodeOrNull<TextureButton>("%SlotButton");
        _quartzMount = GetNodeOrNull<Control>("%QuartzMount");
        _lockOverlay = GetNodeOrNull<TextureRect>("%LockOverlay");
        _selectionGlow = GetNodeOrNull<TextureRect>("%SelectionGlow");

        if (_slotButton != null)
            _slotButton.MouseFilter = MouseFilterEnum.Ignore;

        if (_quartzMount != null)
            _quartzMount.MouseFilter = MouseFilterEnum.Ignore;

        if (_lockOverlay != null)
            _lockOverlay.MouseFilter = MouseFilterEnum.Ignore;

        if (_selectionGlow != null)
            _selectionGlow.MouseFilter = MouseFilterEnum.Ignore;

        ClearQuartzMount();

        if (_selectionGlow != null)
            _selectionGlow.Visible = false;
    }

    public void SetSlot(int slotIndex, bool unlocked, QuartzDefinition? quartz)
    {
        _slotIndex = slotIndex;
        _isUnlocked = unlocked;

        GD.Print($"NOrbmentSlotDisplay: SetSlot index={slotIndex}, unlocked={unlocked}, quartz={(quartz == null ? "none" : quartz.Id)}");

        if (_lockOverlay != null)
            _lockOverlay.Visible = !unlocked;

        if (_quartzMount != null)
            _quartzMount.Visible = unlocked;

        ClearQuartzMount();

        if (!unlocked)
            return;

        if (quartz == null)
            return;

        AddQuartzDisplay(quartz);
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        if (!_isUnlocked)
            return false;

        return NQuartzDisplay.TryReadQuartzDragData(
            data,
            out _,
            out _,
            out _
        );
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        if (!_isUnlocked)
            return;

        if (!NQuartzDisplay.TryReadQuartzDragData(
                data,
                out var quartzId,
                out var source,
                out var sourceSlotIndex))
        {
            return;
        }

        GD.Print($"NOrbmentSlotDisplay: Dropped quartz={quartzId}, source={source}, sourceSlot={sourceSlotIndex}, targetSlot={_slotIndex}");

        QuartzDroppedOnSlot?.Invoke(
            _slotIndex,
            quartzId,
            source,
            sourceSlotIndex
        );
    }

    private void ClearQuartzMount()
    {
        if (_quartzMount == null)
            return;

        foreach (var child in _quartzMount.GetChildren())
        {
            child.QueueFree();
        }
    }

    private void AddQuartzDisplay(QuartzDefinition quartz)
    {
        if (_quartzMount == null)
        {
            GD.PrintErr($"NOrbmentSlotDisplay: Slot {_slotIndex} has no QuartzMount.");
            return;
        }

        var quartzScene = GD.Load<PackedScene>(
            "res://TrailsWithinTheSpireMod/scenes/QuartzDisplay.tscn"
        );

        if (quartzScene == null)
        {
            GD.PrintErr("NOrbmentSlotDisplay: Could not load QuartzDisplay.tscn.");
            return;
        }

        var display = quartzScene.Instantiate<NQuartzDisplay>();

        display.SetQuartz(quartz);
        display.SetDragContextSlot(_slotIndex);
        display.MouseFilter = MouseFilterEnum.Stop;

        _quartzMount.AddChild(display);

        CenterQuartzDisplay(display);
    }

    private void CenterQuartzDisplay(Control display)
    {
        if (_quartzMount == null)
            return;

        var baseSize = new Vector2(SlotQuartzBaseSize, SlotQuartzBaseSize);
        var scaledSize = baseSize * SlotQuartzScale;

        display.SetAnchorsPreset(LayoutPreset.TopLeft);

        display.CustomMinimumSize = baseSize;
        display.Size = baseSize;
        display.Scale = Vector2.One * SlotQuartzScale;

        display.Position = (_quartzMount.Size - scaledSize) / 2f;
    }
}