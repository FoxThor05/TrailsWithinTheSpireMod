using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context; // Added for LocalContext
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players; // Added for Player
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment; // Added for OrbmentManager, BattleOrbmentState, QuartzDatabase, OrbmentCombatState
using Godot;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Character; // Added for GD.Print

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Relics;

[Pool(typeof(TrailsWithinTheSpireModRelicPool))]
public sealed class OrbmentTestRelic : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            return new DynamicVar[]
            {
                // Add DynamicVar entries here
            };
        }
    }

    public override async Task BeforeCombatStart()
    {
        Flash();
        
        // Get the current BattleOrbmentState
        BattleOrbmentState orbmentState = OrbmentManager.Current;

        // Unlock all 6 Orbment slots
        for (int i = orbmentState.UnlockedSlots; i < BattleOrbmentState.MaxSlots; i++)
        {
            orbmentState.UnlockSlot();
            GD.Print($"OrbmentTestRelic: Unlocked Orbment slot {orbmentState.UnlockedSlots}");
        }

        // For testing, let's equip one of each Quartz type into the first available slots
        // This is a temporary measure to simulate "having" quartz for testing purposes.
        // If a proper inventory system is implemented, this logic would change.
        int slotIndex = 0;
        foreach (var quartzDef in QuartzDatabase.All)
        {
            if (slotIndex < BattleOrbmentState.MaxSlots)
            {
                orbmentState.SetSlot(slotIndex, quartzDef.Id);
                GD.Print($"OrbmentTestRelic: Equipped {quartzDef.Id} into slot {slotIndex}");
                slotIndex++;
            }
            else
            {
                GD.PrintErr("OrbmentTestRelic: Ran out of slots to equip all test Quartz.");
                break;
            }
        }

        await Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        // Only reset for the local player
        if (LocalContext.IsMe(player))
        {
            OrbmentCombatState.ResetTurn();
            GD.Print("ARTS_LOG: OrbmentCombatState.ResetTurn() called via OrbmentTestRelic.OnPlayerTurnStart.");
        }
        await Task.CompletedTask;
    }
}
