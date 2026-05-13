using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Powers;

public sealed class ArtCostReductionPower : TrailsWithinTheSpireModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        modifiedCost = originalCost;

        if (Amount <= 0)
            return false;

        if (card.Owner.Creature != Owner)
            return false;

        if (card is not IArtCard)
            return false;

        var pileType = card.Pile?.Type;

        if (pileType != PileType.Hand && pileType != PileType.Play)
            return false;

        if (originalCost <= 0)
            return false;

        var reduction = Math.Min(Amount, (int)originalCost);

        if (reduction <= 0)
            return false;

        modifiedCost = Math.Max(0, originalCost - reduction);

        return true;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (Amount <= 0)
            return;

        var card = cardPlay.Card;

        if (card.Owner.Creature != Owner)
            return;

        if (card is not IArtCard)
            return;

        var originalCost = card.EnergyCost.GetWithModifiers(CostModifiers.Local);

        if (originalCost <= 0)
            return;

        var reduction = Math.Min(Amount, originalCost);

        if (reduction <= 0)
            return;

        for (var i = 0; i < reduction; i++)
        {
            await PowerCmd.Decrement(this);
        }
    }
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side)
            return;

        await PowerCmd.ModifyAmount(choiceContext, this, -Amount, (Creature?)null, null);
    }
}