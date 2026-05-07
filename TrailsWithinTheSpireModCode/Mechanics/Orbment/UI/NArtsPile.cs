using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Localization;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;


namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

public partial class NArtsPile : NCombatCardPile
{

    public static NArtsPile? Instance { get; private set; }
    
    protected override PileType Pile => ArtsCardPile.ArtsPileType;

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

        var border = new Panel { Name = "ArtsButtonBorder", MouseFilter = MouseFilterEnum.Ignore };
        border.SetAnchorsPreset(LayoutPreset.FullRect);
        var borderStyle = new StyleBoxFlat {
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

        var titleLabel = new Label();
        titleLabel.Text = "ORBAL ARTS";
        titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
        titleLabel.VerticalAlignment = VerticalAlignment.Center;
        titleLabel.SetAnchorsPreset(LayoutPreset.FullRect);
        titleLabel.MouseFilter = MouseFilterEnum.Ignore;
        titleLabel.AddThemeColorOverride("font_color", textColor);
        titleLabel.AddThemeConstantOverride("outline_size", 8);
        titleLabel.AddThemeColorOverride("font_outline_color", strokeColor);
        titleLabel.AddThemeFontSizeOverride("font_size", 16);
        AddChild(titleLabel);
    }
    
    private async Task OnCastPressed()
    {
        if (_localPlayer == null) return;

        var totals = OrbmentManager.Current.GetElementTotals();
        var availableArts = ArtResolver.GetUnlockedArts(totals)
            .Where(art => {
                string? reason;
                return OrbmentCastService.CanCastArt(art.Id, out reason);
            })
            .Select(art =>
            {
                ArtCardFactory.TryCreate(art.Id, _localPlayer, out var card);
                return card;
            })
            .Where(card => card is IArtCard)
            .Cast<CardModel>()
            .ToList();

        if (availableArts.Count == 0) return;

        var prefs = new CardSelectorPrefs(new LocString("cards", "ChooseAnOrbalArt"), 1)
        {
            RequireManualConfirmation = true,
            Cancelable = true,
            PretendCardsCanBePlayed = true
        };

        var player = CombatManager.Instance.DebugOnlyGetState()?.Players.FirstOrDefault(p => LocalContext.IsMe(p));
        if (player == null)
        {
            GD.PrintErr("NArtsScreenOverlay: LocalPlayer is null when trying to cast Art.");
            return;
        }
        var choiceContext = new ThrowingPlayerChoiceContext();


        var selectedCards = await CardSelectCmd.FromSimpleGrid(
            choiceContext, 
            availableArts,
            _localPlayer,
            prefs
        );

        var selectedCard = selectedCards.FirstOrDefault();

        if (selectedCard is IArtCard selectedArtCard)
        {
            await OrbmentCastService.CastArt(_localPlayer, selectedArtCard.ArtId);
            NCapstoneContainer.Instance?.Close();
        }
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

            if (container.GetNodeOrNull<NOrbmentButton>("NOrbmentButton") == null)
            {
                var orbmentButton = NOrbmentButton.Create();
                container.AddChild(orbmentButton);
                orbmentButton.Position = new Vector2(0, 500);
                orbmentButton.Initialize(player);
                GD.Print("ARTS_LOG: Orbment Button injected via Overlay initialization.");
            }
        }

        if (_pile != null)
        {
            _pile.ContentsChanged += HandleContentsChanged;
            HandleContentsChanged();
            this.Visible = true;
            this.Enable();
        }
    }

    private void HandleContentsChanged()
    {
        if (_artsCountLabel == null) _artsCountLabel = GetNodeOrNull<TrailsMegaLabel>("%ArtsCountLabel");
        
        if (_pile != null && _artsCountLabel != null)
            _artsCountLabel.SetTextAutoSize(_pile.Cards.Count.ToString());
    }
    
    protected override void OnRelease()
    {
        if (_pile != null)
        {
            NCapstoneContainer.Instance?.Close(); 
        
            _activeScreen = NCardPileScreen.ShowScreen(_pile, new string[] { "ui_cancel" });
        }
    }

    public override void _ExitTree()
    {
        if (_pile != null) _pile.ContentsChanged -= HandleContentsChanged;
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
        this.Visible = true;
    }
}
