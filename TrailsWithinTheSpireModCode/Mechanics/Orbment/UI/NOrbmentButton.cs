using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

public partial class NOrbmentButton : NButton
{
    private const string OrbmentButtonIconPath =
        "res://TrailsWithinTheSpireMod/assets/orbment_button.png";

    private static readonly Vector2 ButtonSize = new(56, 56);
    private static readonly HoverTip OrbmentHoverTip = new(
        new LocString("cards", "ORBMENT_BUTTON.title"),
        new LocString("cards", "ORBMENT_BUTTON.description")
    );
    private Player? _localPlayer;
    private TextureRect? _icon;
    private bool _isBuilt;

    public static NOrbmentButton Create()
    {
        return new NOrbmentButton
        {
            Name = "NOrbmentButton",
            CustomMinimumSize = ButtonSize,
            Size = ButtonSize,
            Visible = true,
            MouseFilter = MouseFilterEnum.Stop,
            ClipContents = true
        };
    }

    public override void _Ready()
    {
        if (_isBuilt)
            return;

        _isBuilt = true;

        // NButton explicitly requires this instead of base._Ready().
        ConnectSignals();

        CustomMinimumSize = ButtonSize;
        Size = ButtonSize;
        ClipContents = true;
        MouseFilter = MouseFilterEnum.Stop;

        _icon = new TextureRect
        {
            Name = "OrbmentIcon",
            MouseFilter = MouseFilterEnum.Ignore,
            CustomMinimumSize = Vector2.Zero,
            Size = ButtonSize,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
        };

        _icon.SetAnchorsPreset(LayoutPreset.FullRect);
        _icon.SetOffsetsPreset(LayoutPreset.FullRect);

        var texture = GD.Load<Texture2D>(OrbmentButtonIconPath);

        if (texture == null)
        {
            GD.PrintErr($"ORBMENT_LOG: Could not load Orbment button icon at '{OrbmentButtonIconPath}'.");
        }
        else
        {
            _icon.Texture = texture;
        }

        AddChild(_icon);

        Released += OnButtonPressed;

        GD.Print("ORBMENT_LOG: NOrbmentButton _Ready completed.");
    }

    public void Initialize()
    {
        Visible = true;
        Enable();

        GD.Print("ORBMENT_LOG: NOrbmentButton initialized without player.");
    }

    public void Initialize(Player player)
    {
        _localPlayer = player;
        Initialize();

        GD.Print("ORBMENT_LOG: NOrbmentButton initialized with player.");
    }

    private void OnButtonPressed(NClickableControl control)
    {
        if (CombatManager.Instance != null && !CombatManager.Instance.IsOverOrEnding)
        {
            GD.Print("ORBMENT_LOG: Orbment screen cannot be opened during combat.");
            return;
        }

        if (NOverlayStack.Instance == null)
        {
            GD.PrintErr("ORBMENT_LOG: NOverlayStack.Instance is null.");
            return;
        }

        var orbmentScreenScene = GD.Load<PackedScene>(
            "res://TrailsWithinTheSpireMod/scenes/OrbmentScreen.tscn"
        );

        if (orbmentScreenScene == null)
        {
            GD.PrintErr("ORBMENT_LOG: Failed to load OrbmentScreen.tscn.");
            return;
        }

        var orbmentScreenInstance =
            orbmentScreenScene.Instantiate<NOrbmentOverlayScreen>();

        NOverlayStack.Instance.Push(orbmentScreenInstance);
    }

    protected override void OnFocus()
    {
        base.OnFocus();

        var hoverTipSet = NHoverTipSet.CreateAndShow(this, OrbmentHoverTip);
        hoverTipSet?.SetGlobalPosition(
            GlobalPosition + new Vector2(Size.X - hoverTipSet.Size.X, Size.Y + 20f)
        );

        if (_icon == null)
            return;

        _icon.PivotOffset = _icon.Size / 2f;

        var tween = CreateTween();
        tween.TweenProperty(_icon, "scale", Vector2.One * 1.12f, 0.1);
    }

    protected override void OnUnfocus()
    {
        base.OnUnfocus();

        NHoverTipSet.Remove(this);

        if (_icon == null)
            return;

        var tween = CreateTween();
        tween.TweenProperty(_icon, "scale", Vector2.One, 0.1);
    }}