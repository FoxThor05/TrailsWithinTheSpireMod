using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using System;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Relics;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public static class QuartzEffectDispatcher
{
    private const int Attack1StrengthBonus = 1;

    private const int Hp1MaxHpBonus = 4;

    private static BattleOrbment? _activeBattleOrbment;

    private static int _appliedMaxHpBonus;

    public static void RegisterBattleOrbment(BattleOrbment battleOrbment)
    {
        _activeBattleOrbment = battleOrbment;
    }

    public static async Task ApplyPassiveEffectsFromActiveBattleOrbment()
    {
        if (_activeBattleOrbment == null)
            return;

        await ApplyPassiveEffects(_activeBattleOrbment);
    }

    public static async Task ApplyPassiveEffects(BattleOrbment battleOrbment)
    {
        RegisterBattleOrbment(battleOrbment);

        var creature = battleOrbment.Owner?.Creature;

        if (creature == null)
        {
            GD.PrintErr("ORBMENT_LOG: BattleOrbment has no owner creature; cannot apply passive Quartz effects.");
            return;
        }

        await ApplyHpQuartzPassive(battleOrbment, creature);
    }

    public static async Task TriggerCombatStartEffects(BattleOrbment battleOrbment, AbstractRoom room)
    {
        RegisterBattleOrbment(battleOrbment);

        if (room is not CombatRoom)
            return;

        await ApplyPassiveEffects(battleOrbment);

        var creature = battleOrbment.Owner.Creature;
        var equippedQuartz = OrbmentManager.GetEquippedQuartz();

        var attack1Count = equippedQuartz.Count(q => q.Id == "attack_1");

        if (attack1Count > 0)
        {
            var strengthAmount = attack1Count * Attack1StrengthBonus;

            battleOrbment.Flash();

            await PowerCmd.Apply<StrengthPower>(
                new ThrowingPlayerChoiceContext(),
                creature,
                strengthAmount,
                creature,
                null
            );

            GD.Print($"ORBMENT_LOG: BattleOrbment applied +{strengthAmount} Strength from Attack Quartz.");
        }
    }

    private static async Task ApplyHpQuartzPassive(BattleOrbment battleOrbment, Creature creature)
    {
        var desiredMaxHpBonus = GetDesiredMaxHpBonus();
        var delta = desiredMaxHpBonus - _appliedMaxHpBonus;

        if (delta == 0)
            return;

        var oldMaxHp = creature.MaxHp;
        var oldCurrentHp = creature.CurrentHp;

        var targetMaxHp = Math.Max(1, creature.MaxHp + delta);

        await CreatureCmd.SetMaxHp(creature, targetMaxHp);

        if (creature.CurrentHp > creature.MaxHp)
            await CreatureCmd.SetCurrentHp(creature, creature.MaxHp);

        _appliedMaxHpBonus = desiredMaxHpBonus;

        battleOrbment.Flash();

        GD.Print(
            $"ORBMENT_LOG: HP Quartz passive updated. " +
            $"DesiredBonus={desiredMaxHpBonus}, Delta={delta}, " +
            $"MaxHP {oldMaxHp}->{creature.MaxHp}, CurrentHP {oldCurrentHp}->{creature.CurrentHp}."
        );
    }

    private static int GetDesiredMaxHpBonus()
    {
        var hp1Count = OrbmentManager.GetEquippedQuartz().Count(q => q.Id == "hp_1");

        return hp1Count * Hp1MaxHpBonus;
    }
}