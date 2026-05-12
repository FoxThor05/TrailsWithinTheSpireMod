using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;
using System.Threading.Tasks;
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

        QuartzEffectDispatcher.RegisterBattleOrbment(this);
        await QuartzEffectDispatcher.ApplyPassiveEffects(this);
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        QuartzEffectDispatcher.RegisterBattleOrbment(this);
        await QuartzEffectDispatcher.ApplyPassiveEffects(this);

        if (room is CombatRoom)
        {
            OrbmentCombatState.ResetCombat();
            await QuartzEffectDispatcher.TriggerCombatStartEffects(this, room);
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (LocalContext.IsMe(player))
            OrbmentCombatState.ResetTurn();

        await Task.CompletedTask;
    }
}