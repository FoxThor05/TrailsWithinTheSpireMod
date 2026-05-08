using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public sealed class SelectArtToCastAction : GameAction
{
    private readonly ulong _ownerId;
    private readonly int _playerNetId;
    public override ulong OwnerId => _ownerId;

    public override GameActionType ActionType => GameActionType.Any;

    public SelectArtToCastAction(ulong ownerId, int playerNetId)
    {
        _ownerId = ownerId;
        _playerNetId = playerNetId;
    }

    protected override async Task ExecuteAction()
    {
        GD.Print("ARTS_LOG: SelectArtToCastAction.ExecuteAction started.");

        var combatState = CombatManager.Instance.DebugOnlyGetState();
        if (combatState == null)
        {
            GD.Print("ARTS_LOG: ERR - combatState is null in SelectArtToCastAction.");
            return;
        }

        var player = combatState.Players.FirstOrDefault(p => (int)p.NetId == _playerNetId);
        if (player == null)
        {
            GD.Print($"ARTS_LOG: ERR - Player with NetId {_playerNetId} not found in SelectArtToCastAction.");
            return;
        }

        var artsPile = ArtsCardPile.ArtsPileType.GetPile(player);
        if (artsPile == null)
        {
            GD.Print("ARTS_LOG: ERR - ArtsCardPile not found for the player.");
            return;
        }

        var totals = OrbmentManager.Current.GetElementTotals();
        
        var availableArts = artsPile.Cards
            .Where(card => card is IArtCard)
            .Cast<IArtCard>()
            .Where(artCard => OrbmentCastService.CanCastArt(artCard.ArtId, out _))
            .Cast<CardModel>() 
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

        var choiceContext = new GameActionPlayerChoiceContext(this);

        GD.Print("ARTS_LOG: SelectArtToCastAction: Opening Selection Grid with cards from ArtsCardPile...");
        var selectedCards = await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            availableArts,
            player,
            prefs
        );

        var selectedCard = selectedCards.FirstOrDefault();
        if (selectedCard is IArtCard selectedArtCard)
        {
            GD.Print($"ARTS_LOG: SelectArtToCastAction: Casting Art ID: {selectedArtCard.ArtId}");
            await OrbmentCastService.CastArt(player, selectedArtCard.ArtId);
            NCapstoneContainer.Instance?.Close();
        }
        else
        {
            GD.Print("ARTS_LOG: SelectArtToCastAction: No art selected or invalid art card.");
        }
    }

    public override INetAction ToNetAction()
    {
        return null;
    }
}
