using Godot;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

public partial class NArtsPile : NCombatCardPile
{
    public static NArtsPile? Instance { get; private set; }

    protected override PileType Pile => ArtsCardPile.ArtsPileType;

    private const string OrbalArtsButtonIconPath =
        "res://TrailsWithinTheSpireMod/assets/orbal_arts_button.png";

    private const string ElementalTotalsPanelName = "OrbalArtsElementalTotalsPanel";

    private static readonly Vector2 ButtonSize = new(92f, 92f);
    private static readonly Vector2 CountBadgeSize = new(46f, 46f);

    private CardPile? _pile;
    private Player? _localPlayer;

    private TextureRect? _icon;
    private Control? _countContainer;
    private TextureRect? _countBackground;
    private TrailsMegaLabel? _countLabel;

    private int _lastDisplayedRemainingCasts = -1;

    public static AddedNode<NCombatPilesContainer, NArtsPile> OverlayNode = new(c =>
    {
        var overlayScene = GD.Load<PackedScene>("res://TrailsWithinTheSpireMod/scenes/ArtsPile.tscn");
        var overlayInstance = overlayScene.Instantiate<NArtsPile>();

        overlayInstance.Name = "_ArtsPile";
        Instance = overlayInstance;

        overlayInstance.CustomMinimumSize = ButtonSize;
        overlayInstance.Size = ButtonSize;

        overlayInstance.Position = new Vector2(18f, 670f);

        return overlayInstance;
    });

    public override void _Ready()
    {
        ConnectSignals();

        Instance = this;

        CustomMinimumSize = ButtonSize;
        Size = ButtonSize;
        ClipContents = false;
        MouseFilter = MouseFilterEnum.Stop;

        _icon = GetNodeOrNull<TextureRect>("Icon");
        _countContainer = GetNodeOrNull<Control>("CountContainer");
        _countBackground = GetNodeOrNull<TextureRect>("CountContainer/Background");
        _countLabel = GetNodeOrNull<TrailsMegaLabel>("CountContainer/Count");

        SetupIcon();
        SetupCountBadge();

        RefreshCastCount();

        OrbmentCombatState.StateChanged += RefreshCastCount;
    }

    public override void Initialize(Player player)
    {
        _localPlayer = player;
        _pile = ArtsCardPile.ArtsPileType.GetPile(player);

        if (_pile != null)
            _pile.ContentsChanged += HandleContentsChanged;

        Visible = true;
        Enable();

        RefreshCastCount();

        GD.Print("ARTS_LOG: Single Orbal Arts button initialized.");
    }

    private void SetupIcon()
    {
        if (_icon == null)
        {
            GD.PrintErr("ARTS_LOG: ArtsPile Icon node not found.");
            return;
        }

        _icon.Visible = true;
        _icon.MouseFilter = MouseFilterEnum.Ignore;
        _icon.CustomMinimumSize = Vector2.Zero;
        _icon.Size = ButtonSize;
        _icon.SetAnchorsPreset(LayoutPreset.FullRect);
        _icon.SetOffsetsPreset(LayoutPreset.FullRect);
        _icon.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        _icon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;

        var texture = GD.Load<Texture2D>(OrbalArtsButtonIconPath);

        if (texture == null)
        {
            GD.PrintErr($"ARTS_LOG: Could not load Orbal Arts button icon at '{OrbalArtsButtonIconPath}'.");
            return;
        }

        _icon.Texture = texture;
    }

    private void SetupCountBadge()
    {
        if (_countContainer != null)
        {
            _countContainer.Visible = true;
            _countContainer.MouseFilter = MouseFilterEnum.Ignore;
            _countContainer.CustomMinimumSize = CountBadgeSize;
            _countContainer.Size = CountBadgeSize;
            _countContainer.SetAnchorsPreset(LayoutPreset.BottomRight);

            _countContainer.OffsetLeft = -CountBadgeSize.X + 5f;
            _countContainer.OffsetTop = -CountBadgeSize.Y + 5f;
            _countContainer.OffsetRight = 10f;
            _countContainer.OffsetBottom = 10f;
        }

        if (_countBackground != null)
        {
            _countBackground.Visible = true;
            _countBackground.MouseFilter = MouseFilterEnum.Ignore;
            _countBackground.CustomMinimumSize = CountBadgeSize;
            _countBackground.Size = CountBadgeSize;
            _countBackground.SetAnchorsPreset(LayoutPreset.FullRect);
            _countBackground.SetOffsetsPreset(LayoutPreset.FullRect);
            _countBackground.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
            _countBackground.StretchMode = TextureRect.StretchModeEnum.Scale;
        }

        if (_countLabel != null)
        {
            _countLabel.Visible = true;
            _countLabel.MouseFilter = MouseFilterEnum.Ignore;
            _countLabel.SetAnchorsPreset(LayoutPreset.FullRect);
            _countLabel.SetOffsetsPreset(LayoutPreset.FullRect);
            _countLabel.HorizontalAlignment = HorizontalAlignment.Center;
            _countLabel.VerticalAlignment = VerticalAlignment.Center;
            _countLabel.AddThemeFontSizeOverride("font_size", 24);
            _countLabel.AddThemeConstantOverride("outline_size", 6);
            _countLabel.AddThemeColorOverride("font_outline_color", Colors.Black);
        }
    }

    private void HandleContentsChanged()
    {
        RefreshCastCount();
    }

    public void RefreshCastCount()
    {
        var remainingCasts = OrbmentCombatState.RemainingCastsThisTurn;

        if (_lastDisplayedRemainingCasts == remainingCasts)
            return;

        _lastDisplayedRemainingCasts = remainingCasts;

        if (_countLabel != null)
            _countLabel.SetTextAutoSize(remainingCasts.ToString());
    }

    public override void _Process(double delta)
    {
        RefreshCastCount();
    }

    protected override void OnRelease()
    {
        if (_localPlayer == null)
        {
            GD.PrintErr("ARTS_LOG: Cannot open Orbal Arts screen because _localPlayer is null.");
            return;
        }

        TaskHelper.RunSafely(OpenOrbalArtsSelectionScreen());
    }

    private async Task OpenOrbalArtsSelectionScreen()
    {
        if (_localPlayer == null)
            return;

        var artCards = new List<CardModel>();
        var selectableCards = new HashSet<CardModel>();

        var totals = OrbmentManager.Current.GetElementTotals();

        foreach (var art in ArtDatabase.All)
        {
            if (!ArtCardFactory.TryCreate(art.Id, _localPlayer, out var card) || card == null)
            {
                GD.PrintErr($"ARTS_LOG: Could not create card for Art '{art.Id}'.");
                continue;
            }

            artCards.Add(card);

            if (ArtResolver.MeetsRequirements(art, totals) &&
                OrbmentCastService.CanCastArt(art.Id, out _))
            {
                selectableCards.Add(card);
            }
        }

        if (artCards.Count == 0)
        {
            GD.PrintErr("ARTS_LOG: No Art cards could be created.");
            return;
        }

        var prefs = new CardSelectorPrefs(new LocString("cards", "ChooseAnOrbalArt"), 0, 1)
        {
            RequireManualConfirmation = true,
            Cancelable = true,
            PretendCardsCanBePlayed = true
        };

        NCapstoneContainer.Instance?.Close();

        var screen = NSimpleCardSelectScreen.Create(artCards, prefs);

        NOrbalArtsSelectionRegistry.Register(screen, artCards, selectableCards);

        NOverlayStack.Instance.Push(screen);

        Callable.From(() => AddOrRefreshElementalTotalsPanel(screen)).CallDeferred();

        IEnumerable<CardModel> selectedCards;

        try
        {
            selectedCards = await screen.CardsSelected();
        }
        finally
        {
            NOrbalArtsSelectionRegistry.Unregister(screen);
        }

        var selectedCard = selectedCards.FirstOrDefault();

        if (selectedCard == null)
        {
            GD.Print("ARTS_LOG: Orbal Arts screen closed without selecting an Art.");
            return;
        }

        if (selectedCard is not IArtCard selectedArtCard)
            return;

        if (!OrbmentCastService.CanCastArt(selectedArtCard.ArtId, out var failureReason))
        {
            GD.Print($"ARTS_LOG: Cannot cast {selectedArtCard.ArtId}: {failureReason}");
            return;
        }

        var result = await OrbmentCastService.CastArt(_localPlayer, selectedArtCard.ArtId);

        GD.Print($"ARTS_LOG: {result}");

        RefreshCastCount();
    }

    private static void AddOrRefreshElementalTotalsPanel(NSimpleCardSelectScreen screen)
    {
        if (!GodotObject.IsInstanceValid(screen))
            return;

        var existing = screen.GetNodeOrNull<Control>(ElementalTotalsPanelName);

        if (existing != null)
        {
            NOrbalArtsElementalTotalsPanel.Refresh(existing);
            return;
        }

        var panel = NOrbalArtsElementalTotalsPanel.Create();
        panel.Name = ElementalTotalsPanelName;

        screen.AddChild(panel);

        panel.SetAnchorsPreset(LayoutPreset.TopRight);

        panel.OffsetLeft = -260f;
        panel.OffsetRight = -70f;
        panel.OffsetTop = 260f;
        panel.OffsetBottom = 600f;

        NOrbalArtsElementalTotalsPanel.Refresh(panel);

        GD.Print("ARTS_LOG: Added elemental totals panel to Orbal Arts selection screen.");
    }

    public override void _ExitTree()
    {
        OrbmentCombatState.StateChanged -= RefreshCastCount;

        if (_pile != null)
            _pile.ContentsChanged -= HandleContentsChanged;

        NHoverTipSet.Remove(this);

        base._ExitTree();
    }

    protected override void OnFocus()
    {
        base.OnFocus();

        var hoverTip = new HoverTip(
            new LocString("cards", "ORBAL_ARTS_BUTTON.title"),
            new LocString("cards", "ORBAL_ARTS_BUTTON.description")
        );

        var hoverTipSet = NHoverTipSet.CreateAndShow(this, hoverTip);

        hoverTipSet?.SetGlobalPosition(
            GlobalPosition + new Vector2(Size.X + 14f, -44f)
        );

        var tween = CreateTween();
        tween.TweenProperty(this, "scale", new Vector2(1.1f, 1.1f), 0.1);
    }

    protected override void OnUnfocus()
    {
        base.OnUnfocus();

        NHoverTipSet.Remove(this);

        var tween = CreateTween();
        tween.TweenProperty(this, "scale", Vector2.One, 0.1);
    }

    public override void AnimIn()
    {
        base.AnimIn();
        Visible = true;
    }
}