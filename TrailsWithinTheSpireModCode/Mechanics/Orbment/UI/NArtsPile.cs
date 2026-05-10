using Godot;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

public partial class NArtsPile : NCombatCardPile
{
    public static NArtsPile? Instance { get; private set; }

    protected override PileType Pile => ArtsCardPile.ArtsPileType;

    private const string ElementalTotalsPanelName = "OrbalArtsElementalTotalsPanel";

    private CardPile? _pile;
    private Player? _localPlayer;
    private TrailsMegaLabel? _artsCountLabel;
    private NCardPileScreen? _activeScreen;

    public static AddedNode<NCombatPilesContainer, NArtsPile> OverlayNode = new(c =>
    {
        var overlayScene = GD.Load<PackedScene>("res://TrailsWithinTheSpireMod/scenes/ArtsPile.tscn");
        var overlayInstance = overlayScene.Instantiate<NArtsPile>();

        overlayInstance.Name = "_ArtsPile";

        Instance = overlayInstance;

        overlayInstance.CustomMinimumSize = new Vector2(150, 60);
        overlayInstance.Position = new Vector2(0, 600);

        return overlayInstance;
    });

    public override void _Ready()
    {
        ConnectSignals();

        Instance = this;

        GetNodeOrNull<TextureRect>("Icon")?.Hide();
        GetNodeOrNull<Control>("CountContainer")?.Hide();

        var bgColor = Color.FromHtml("7b1b16");
        var textColor = Color.FromHtml("ffeecd");
        var strokeColor = Color.FromHtml("3d0c08");

        var body = new ColorRect
        {
            Name = "ArtsButtonBackground",
            Color = bgColor,
            MouseFilter = MouseFilterEnum.Ignore
        };

        body.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(body);
        MoveChild(body, 0);

        var border = new Panel
        {
            Name = "ArtsButtonBorder",
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

        var titleLabel = new Label
        {
            Name = "ArtsButtonLabel",
            Text = "ORBAL ARTS",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            MouseFilter = MouseFilterEnum.Ignore
        };

        titleLabel.SetAnchorsPreset(LayoutPreset.FullRect);
        titleLabel.AddThemeColorOverride("font_color", textColor);
        titleLabel.AddThemeConstantOverride("outline_size", 8);
        titleLabel.AddThemeColorOverride("font_outline_color", strokeColor);
        titleLabel.AddThemeFontSizeOverride("font_size", 16);

        AddChild(titleLabel);
    }

    public override void Initialize(Player player)
    {
        _localPlayer = player;
        _pile = ArtsCardPile.ArtsPileType.GetPile(player);

        var container = GetParent();

        if (container != null)
        {
            if (container.GetNodeOrNull<NCastArtButton>("NCastArtButton") == null)
            {
                var castButton = NCastArtButton.Create();

                container.AddChild(castButton);

                castButton.Position = new Vector2(0, 700);
                castButton.Initialize(player);

                GD.Print("ARTS_LOG: Cast Button injected via Overlay initialization.");
            }
        }

        if (_pile != null)
        {
            _pile.ContentsChanged += HandleContentsChanged;
            HandleContentsChanged();

            Visible = true;
            Enable();
        }
        else
        {
            GD.PrintErr("ARTS_LOG: Arts pile was null during NArtsPile.Initialize.");
        }
    }

    private void HandleContentsChanged()
    {
        if (_artsCountLabel == null)
            _artsCountLabel = GetNodeOrNull<TrailsMegaLabel>("%ArtsCountLabel");

        if (_pile != null && _artsCountLabel != null)
            _artsCountLabel.SetTextAutoSize(_pile.Cards.Count.ToString());
    }

    protected override void OnRelease()
    {
        if (_pile == null)
        {
            GD.PrintErr("ARTS_LOG: Cannot open Orbal Arts screen because _pile is null.");
            return;
        }

        NCapstoneContainer.Instance?.Close();

        _activeScreen = NCardPileScreen.ShowScreen(_pile, new[] { "ui_cancel" });

        if (_activeScreen != null)
        {
            Callable.From(() => AddOrRefreshElementalTotalsPanel(_activeScreen)).CallDeferred();
        }
        else
        {
            GD.PrintErr("ARTS_LOG: NCardPileScreen.ShowScreen returned null for Orbal Arts.");
        }
    }

    private static void AddOrRefreshElementalTotalsPanel(NCardPileScreen screen)
    {
        if (!GodotObject.IsInstanceValid(screen))
            return;

        if (screen.Pile == null || screen.Pile.Type != ArtsCardPile.ArtsPileType)
            return;

        var existing = screen.GetNodeOrNull<Control>(NOrbalArtsElementalTotalsPanel.NodeName);

        if (existing != null)
        {
            NOrbalArtsElementalTotalsPanel.Refresh(existing);
            GD.Print("ARTS_LOG: Refreshed existing elemental totals panel.");
            return;
        }

        var panel = NOrbalArtsElementalTotalsPanel.Create();

        screen.AddChild(panel);

        panel.SetAnchorsPreset(LayoutPreset.TopRight);

        // Right-side column placement.
        panel.OffsetLeft = -260f;
        panel.OffsetRight = -70f;
        panel.OffsetTop = 260f;
        panel.OffsetBottom = 600f;

        NOrbalArtsElementalTotalsPanel.Refresh(panel);

        GD.Print("ARTS_LOG: Added elemental totals panel directly from NArtsPile.OnRelease.");
    }
    public override void _ExitTree()
    {
        if (_pile != null)
            _pile.ContentsChanged -= HandleContentsChanged;

        base._ExitTree();
    }

    protected override void OnFocus()
    {
        base.OnFocus();

        var tween = CreateTween();
        tween.TweenProperty(this, "scale", new Vector2(1.1f, 1.1f), 0.1);
    }

    protected override void OnUnfocus()
    {
        base.OnUnfocus();

        var tween = CreateTween();
        tween.TweenProperty(this, "scale", Vector2.One, 0.1);
    }

    public override void AnimIn()
    {
        base.AnimIn();
        Visible = true;
    }
}