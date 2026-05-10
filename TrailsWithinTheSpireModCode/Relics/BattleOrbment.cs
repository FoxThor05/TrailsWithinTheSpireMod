using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Relics;
using System.Threading.Tasks;
using Godot;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Relics;

public sealed class BattleOrbment : TrailsWithinTheSpireModRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override async Task AfterObtained()
    {
        OrbmentRelicFields.UnlockedSlots[this] = 1;

        if (string.IsNullOrWhiteSpace(OrbmentRelicFields.EquippedQuartz[this]))
            OrbmentRelicFields.EquippedQuartz[this] = OrbmentRelicFields.EmptySlotsEncoded;

        if (string.IsNullOrWhiteSpace(OrbmentRelicFields.OwnedQuartz[this]))
            OrbmentRelicFields.OwnedQuartz[this] = "";

        await Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (LocalContext.IsMe(player))
        {
            OrbmentCombatState.ResetTurn();
            GD.Print("ARTS_LOG: OrbmentCombatState.ResetTurn() called via BattleOrbment.AfterPlayerTurnStart.");
        }

        await Task.CompletedTask;
    }
}