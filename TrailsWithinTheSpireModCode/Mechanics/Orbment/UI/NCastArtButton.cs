using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Runs;
namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

public partial class NCastArtButton : NButton
{
    private Player? _localPlayer;
    private Label? _label;

    public static NCastArtButton Create()
    {
        var button = new NCastArtButton();
        button.Name = "NCastArtButton";
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
        _label.Text = "CAST ARTS";
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

    private async void OnButtonPressed(NClickableControl control)
    {
        if (_localPlayer == null)
            return;

        await OpenArtSelectionLocal();
    }
    
    private async Task OpenArtSelectionLocal()
    {
        if (_localPlayer == null)
            return;

        var artsPile = ArtsCardPile.ArtsPileType.GetPile(_localPlayer);

        if (artsPile == null)
        {
            GD.Print("ARTS_LOG: ERR - ArtsCardPile not found for the player.");
            return;
        }

        var availableArts = artsPile.Cards
            .OfType<CardModel>()
            .Where(card => card is IArtCard artCard &&
                           OrbmentCastService.CanCastArt(artCard.ArtId, out _))
            .ToList();

        if (availableArts.Count == 0)
        {
            GD.Print("ARTS_LOG: No arts available to cast from ArtsCardPile.");
            return;
        }

        var prefs = new CardSelectorPrefs(new LocString("cards", "ChooseAnOrbalArt"), 1)
        {
            RequireManualConfirmation = true,
            Cancelable = true,
            PretendCardsCanBePlayed = true
        };

        NPlayerHand.Instance?.CancelAllCardPlay();

        var screen = NSimpleCardSelectScreen.Create(
            availableArts,
            prefs
        );

        NOverlayStack.Instance.Push(screen);

        var selectedCard = (await screen.CardsSelected()).FirstOrDefault();

        if (selectedCard is not IArtCard selectedArtCard)
            return;

        GD.Print($"ARTS_LOG: Casting Art ID: {selectedArtCard.ArtId}");

        var result = await OrbmentCastService.CastArt(
            _localPlayer,
            selectedArtCard.ArtId
        );

        GD.Print($"ARTS_LOG: {result}");

        NCapstoneContainer.Instance?.Close();
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
