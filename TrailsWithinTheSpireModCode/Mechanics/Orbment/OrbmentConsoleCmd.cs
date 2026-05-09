
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Cards.Arts;
using System.Linq;
using MegaCrit.Sts2.Core.Entities.Players;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public class OrbmentConsoleCmd : AbstractConsoleCmd
{
    public override string CmdName => "orbment";
    public override string Args => "<unlock|equip|totals|arts|cast|addquartz>";
    public override string Description => "Debug commands for the Battle Orbment system.";
    public override bool IsNetworked => false;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        var totals = OrbmentManager.Current.GetElementTotals();

        if (args.Length == 0)
            return new CmdResult(false, "Usage: orbment unlock | orbment equip <quartzId> <slotIndex> | orbment totals");

        switch (args[0].ToLowerInvariant())
        {
            case "unlock":
                OrbmentManager.Current.UnlockSlot();
                return new CmdResult(true, $"Unlocked slots: {OrbmentManager.Current.UnlockedSlots}");

            case "equip":
                if (args.Length < 3)
                    return new CmdResult(false, "Usage: orbment equip <quartzId> <slotIndex>");

                var quartz = QuartzDatabase.All.FirstOrDefault(q => q.Id == args[1]);
                if (quartz == null)
                    return new CmdResult(false, $"Quartz not found: {args[1]}");

                if (!int.TryParse(args[2], out var slotIndex))
                    return new CmdResult(false, "Slot index must be a number.");

                if (!OrbmentManager.Current.EquipQuartz(slotIndex, quartz))
                    return new CmdResult(false, $"Could not equip {quartz.Id} in slot {slotIndex}.");

                return new CmdResult(true, $"Equipped {quartz.Id} in slot {slotIndex}.");

            case "totals":
                if (totals.Count == 0)
                    return new CmdResult(true, "No elemental values.");
                return new CmdResult(true, string.Join(", ", totals.Select(t => $"{t.Key}: {t.Value}")));
            
            case "arts":
                var arts = ArtResolver.GetUnlockedArts(totals);
                if (arts.Count == 0)
                    return new CmdResult(true, "No Arts unlocked.");

                return new CmdResult(true,
                    string.Join(", ", arts.Select(a => a.Id)));
            case "cast":
            {
                if (issuingPlayer == null)
                    return new CmdResult(false, "This command requires a player.");

                if (args.Length < 2)
                    return new CmdResult(false, "Usage: orbment cast <artId>");

                _ = OrbmentCastService.CastArt(issuingPlayer, args[1]);

                return new CmdResult(true, $"Attempted to cast {args[1]}.");
            }
            case "resetturn":
                OrbmentCombatState.ResetTurn();
                return new CmdResult(true, "Orbment turn state reset.");
            case "addquartz":
                if (args.Length < 2)
                    return new CmdResult(false, "Usage: orbment addquartz <quartzId>");

                var ownedQuartz = QuartzDatabase.All.FirstOrDefault(q => q.Id == args[1]);

                if (ownedQuartz == null)
                    return new CmdResult(false, $"Quartz not found: {args[1]}");

                OrbmentManager.AddQuartz(ownedQuartz.Id);

                return new CmdResult(true, $"Added quartz: {ownedQuartz.Id}");
            default:
                return new CmdResult(false, $"Unknown orbment command: {args[0]}");
        }
    }
}