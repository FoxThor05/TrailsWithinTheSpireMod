#nullable enable
using Godot;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

public partial class NOrbmentSlotDisplay : NClickableControl
{
    private int _slotIndex = -1;
    private bool _isUnlocked;

    private TextureButton? _slotButton;
    private Control? _quartzMount;
    private TextureRect? _lockOverlay;
    private TextureRect? _selectionGlow;

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

    private void ClearQuartzMount()
    {
        if (_quartzMount == null)
            return;

        foreach (var child in _quartzMount.GetChildren())
        {
            child.QueueFree();
        }
    }

    private const float SlotQuartzSize = 64f;

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
        display.MouseFilter = MouseFilterEnum.Stop;

        _quartzMount.AddChild(display);

        CenterQuartzDisplay(display);
    }

    private void CenterQuartzDisplay(Control display)
    {
        if (_quartzMount == null)
            return;

        var size = new Vector2(SlotQuartzSize, SlotQuartzSize);

        display.SetAnchorsPreset(LayoutPreset.TopLeft);
        display.CustomMinimumSize = size;
        display.Size = size;

        display.Position = (_quartzMount.Size - size) / 2f;
    }
}