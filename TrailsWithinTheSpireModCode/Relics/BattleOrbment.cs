using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.RestSite;
using System.Collections.Generic;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.RestSite;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Relics;

public sealed class BattleOrbment : TrailsWithinTheSpireModRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override async Task AfterObtained()
    {
        OrbmentRelicFields.Normalize(this);

        QuartzEffectDispatcher.RegisterBattleOrbment(this);
        await QuartzEffectDispatcher.ApplyPassiveEffects(this);
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        OrbmentRelicFields.Normalize(this);

        QuartzEffectDispatcher.RegisterBattleOrbment(this);
        await QuartzEffectDispatcher.ApplyPassiveEffects(this);

        if (room is CombatRoom)
        {
            OrbmentCombatState.ResetCombat();
            await QuartzEffectDispatcher.TriggerCombatStartEffects(this, room);
        }
    }
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player != Owner)
            return false;

        OrbmentRelicFields.Normalize(this);

        options.Add(new UnlockOrbmentSlotRestSiteOption(player, this));

        return true;
    }
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (LocalContext.IsMe(player))
            OrbmentCombatState.ResetTurn();
        await QuartzEffectDispatcher.ApplyTurnStartEpCut();

        await Task.CompletedTask;
    }
}