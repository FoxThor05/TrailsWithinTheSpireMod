using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

public partial class NOrbmentButton : NButton
{
    private Player? _localPlayer;
    private Label? _label;

    public static NOrbmentButton Create()
    {
        var button = new NOrbmentButton();
        button.Name = "NOrbmentButton";
        button.CustomMinimumSize = new Vector2(150, 60);
        return button;
    }

    public override void _Ready()
    {
        this.ConnectSignals(); 
    
        var bgColor = Color.FromHtml("7b1b16"); 
        var textColor = Color.FromHtml("ffeecd"); 
        var strokeColor = Color.FromHtml("3d0c08");

        var body = new ColorRect
        {
            Name = "ButtonBackground",
            Color = bgColor,
            MouseFilter = MouseFilterEnum.Ignore
        };
        body.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(body);
        
        var border = new Panel
        {
            Name = "ButtonBorder",
            MouseFilter = MouseFilterEnum.Ignore
        };
        border.SetAnchorsPreset(LayoutPreset.FullRect);
    
        var borderStyle = new StyleBoxFlat
        {
            DrawCenter = false,
            BorderWidthLeft = 4,
            BorderWidthTop = 4,
            BorderWidthRight = 4,
            BorderWidthBottom = 4,
            BorderColor = strokeColor,
            ExpandMarginLeft = 2,
            ExpandMarginRight = 2,
            ExpandMarginTop = 2,
            ExpandMarginBottom = 2
        };
        border.AddThemeStyleboxOverride("panel", borderStyle);
        AddChild(border);

        _label = new Label();
        _label.Text = "ORBMENT DEVICE";
        _label.HorizontalAlignment = HorizontalAlignment.Center;
        _label.VerticalAlignment = VerticalAlignment.Center;
        _label.SetAnchorsPreset(LayoutPreset.FullRect);
        _label.MouseFilter = MouseFilterEnum.Ignore;

        _label.AddThemeColorOverride("font_color", textColor);
        _label.AddThemeConstantOverride("outline_size", 8);
        _label.AddThemeColorOverride("font_outline_color", strokeColor);
        _label.AddThemeFontSizeOverride("font_size", 16);
    
        AddChild(_label);

        this.Released += OnButtonPressed;
    }

    public void Initialize(Player player)
    {
        _localPlayer = player;
        this.Visible = true;
        this.Enable();
    }

    private void OnButtonPressed(NClickableControl control)
    {
        if (NOverlayStack.Instance != null)
        {
            var orbmentScreenScene = GD.Load<PackedScene>("res://TrailsWithinTheSpireMod/scenes/OrbmentDevice.tscn");
            if (orbmentScreenScene != null)
            {
                var orbmentScreenInstance = orbmentScreenScene.Instantiate<NOrbmentOverlayScreen>();
                NOverlayStack.Instance.Push(orbmentScreenInstance);
            }
            else
            {
                GD.PrintErr("Failed to load OrbmentScreen.tscn for NOrbmentButton.");
            }
        }
    }
    
    protected override void OnFocus()
    {
        base.OnFocus();
        var tween = CreateTween().SetParallel();
        tween.TweenProperty(this, "scale", new Vector2(1.05f, 1.05f), 0.1);
    }

    protected override void OnUnfocus()
    {
        base.OnUnfocus();
        var tween = CreateTween();
        tween.TweenProperty(this, "scale", Vector2.One, 0.1);
    }
}
