using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Cards;

public sealed class Cast : TrailsWithinTheSpireModCard
{
    public Cast()
        : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var totals = OrbmentManager.Current.GetElementTotals();

        var availableArts = ArtResolver.GetUnlockedArts(totals)
            .Where(art => OrbmentCastService.CanCastArt(art.Id, out _))
            .Select(art =>
            {
                ArtCardFactory.TryCreate(art.Id, Owner, out var card);
                return card;
            })
            .Where(card => card is IArtCard)
            .Cast<CardModel>()
            .ToList();

        if (availableArts.Count == 0)
            return;

        var prefs = new CardSelectorPrefs(
            new LocString("cards", "ChooseAnOrbalArt"),
            1
        )
        {
            RequireManualConfirmation = true,
            Cancelable = true,
            PretendCardsCanBePlayed = true
        };

        var selectedCards = await CardSelectCmd.FromSimpleGrid(
            choiceContext, 
            availableArts,
            Owner,
            prefs
        );

        var selectedCard = selectedCards.FirstOrDefault();

        if (selectedCard is not IArtCard selectedArtCard)
            return;

        await OrbmentCastService.CastArt(Owner, selectedArtCard.ArtId);
    }
}   