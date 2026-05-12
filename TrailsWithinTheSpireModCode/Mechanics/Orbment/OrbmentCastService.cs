using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using System.Linq;
using System.Threading.Tasks;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public static class OrbmentCastService
{
    public static bool CanCastArt(string artId, out string failureReason)
    {
        var totals = OrbmentManager.Current.GetElementTotals();
        var unlocked = ArtResolver.GetUnlockedArts(totals);
        var art = unlocked.FirstOrDefault(a => a.Id == artId);

        if (art == null)
        {
            failureReason = $"Art not unlocked: {artId}";
            return false;
        }

        if (OrbmentCombatState.RemainingCastsThisTurn <= 0)
        {
            failureReason = "No casts remaining this turn.";
            return false;
        }

        if (art.IsHealing && OrbmentCombatState.UsedHealingArts.Contains(art.Id))
        {
            failureReason = "This Healing Art has already been used this combat.";
            return false;
        }

        failureReason = "";
        return true;
    }

    public static async Task<string> CastArt(Player player, string artId)
    {
        if (!CanCastArt(artId, out var failureReason))
            return failureReason;

        var totals = OrbmentManager.Current.GetElementTotals();
        var art = ArtResolver.GetUnlockedArts(totals).First(a => a.Id == artId);

        if (!ArtCardFactory.TryCreate(art.Id, player, out var card) || card == null)
            return $"No card mapped for Art: {art.Id}";

        OrbmentCombatState.UseCast();

        if (art.IsHealing)
            OrbmentCombatState.UsedHealingArts.Add(art.Id);

        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);

        return $"Added {art.Id} to hand.";
    }
}