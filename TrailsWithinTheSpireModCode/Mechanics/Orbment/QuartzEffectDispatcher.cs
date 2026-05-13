using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Relics;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public static class QuartzEffectDispatcher
{
    private static BattleOrbment? _activeBattleOrbment;

    public static void RegisterBattleOrbment(BattleOrbment battleOrbment)
    {
        _activeBattleOrbment = battleOrbment;
        OrbmentManager.RegisterBattleOrbment(battleOrbment);
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

        OrbmentCombatState.RefreshMaxCastsForCurrentTurn();
    }

    public static async Task TriggerCombatStartEffects(BattleOrbment battleOrbment, AbstractRoom room)
    {
        RegisterBattleOrbment(battleOrbment);

        if (room is not CombatRoom)
            return;

        await ApplyPassiveEffects(battleOrbment);

        var creature = battleOrbment.Owner.Creature;

        var strengthAmount = GetCombatStartStrengthBonus();

        if (strengthAmount > 0)
        {
            battleOrbment.Flash();

            await PowerCmd.Apply<StrengthPower>(
                new ThrowingPlayerChoiceContext(),
                creature,
                strengthAmount,
                creature,
                (CardModel?)null
            );

            GD.Print($"ORBMENT_LOG: BattleOrbment applied +{strengthAmount} Strength from Attack Quartz.");
        }

        var dexterityAmount = GetCombatStartDexterityBonus();

        if (dexterityAmount > 0)
        {
            battleOrbment.Flash();

            await PowerCmd.Apply<DexterityPower>(
                new ThrowingPlayerChoiceContext(),
                creature,
                dexterityAmount,
                creature,
                (CardModel?)null
            );

            GD.Print($"ORBMENT_LOG: BattleOrbment applied +{dexterityAmount} Dexterity from Defense Quartz.");
        }
    }
    public static int GetEpCutStacksPerTurn()
    {
        var equippedQuartz = OrbmentManager.GetEquippedQuartz();

        var stacks = 0;

        foreach (var quartz in equippedQuartz)
        {
            stacks += quartz.Id switch
            {
                "ep_cut_1" => 1,
                "ep_cut_2" => 2,
                "ep_cut_3" => 3,
                _ => 0
            };
        }

        return stacks;
    }

    public static async Task ApplyTurnStartEpCut()
    {
        if (_activeBattleOrbment?.Owner?.Creature == null)
            return;

        var stacks = GetEpCutStacksPerTurn();

        if (stacks <= 0)
            return;

        var creature = _activeBattleOrbment.Owner.Creature;

        _activeBattleOrbment.Flash();

        await PowerCmd.Apply<ArtCostReductionPower>(
            new ThrowingPlayerChoiceContext(),
            creature,
            stacks,
            creature,
            null
        );

        GD.Print($"ORBMENT_LOG: BattleOrbment applied {stacks} EP Cut stack(s) from EP Cut Quartz.");
    }
    public static int GetAdditionalCastsPerTurn()
    {
        var equippedQuartz = OrbmentManager.GetEquippedQuartz();

        var additionalCasts = 0;

        foreach (var quartz in equippedQuartz)
        {
            additionalCasts += quartz.Id switch
            {
                "cast_1" => 1,
                "cast_2" => 2,
                "cast_3" => 3,
                _ => 0
            };
        }

        return additionalCasts;
    }

    private static async Task ApplyHpQuartzPassive(BattleOrbment battleOrbment, Creature creature)
    {
        OrbmentRelicFields.Normalize(battleOrbment);

        var desiredMaxHpBonus = GetDesiredMaxHpBonus();
        var alreadyAppliedMaxHpBonus = OrbmentRelicFields.AppliedMaxHpBonus[battleOrbment];

        var delta = desiredMaxHpBonus - alreadyAppliedMaxHpBonus;

        if (delta == 0)
            return;

        var oldMaxHp = creature.MaxHp;
        var oldCurrentHp = creature.CurrentHp;

        var targetMaxHp = Math.Max(1, creature.MaxHp + delta);

        await CreatureCmd.SetMaxHp(creature, targetMaxHp);

        if (creature.CurrentHp > creature.MaxHp)
            await CreatureCmd.SetCurrentHp(creature, creature.MaxHp);

        OrbmentRelicFields.AppliedMaxHpBonus[battleOrbment] = desiredMaxHpBonus;

        battleOrbment.Flash();

        GD.Print(
            $"ORBMENT_LOG: HP Quartz passive updated. " +
            $"DesiredBonus={desiredMaxHpBonus}, Delta={delta}, " +
            $"MaxHP {oldMaxHp}->{creature.MaxHp}, CurrentHP {oldCurrentHp}->{creature.CurrentHp}."
        );
    }

    private static int GetDesiredMaxHpBonus()
    {
        var equippedQuartz = OrbmentManager.GetEquippedQuartz();

        var maxHpBonus = 0;

        foreach (var quartz in equippedQuartz)
        {
            maxHpBonus += quartz.Id switch
            {
                "hp_1" => 4,
                "hp_2" => 8,
                "hp_3" => 12,
                _ => 0
            };
        }

        return maxHpBonus;
    }

    private static int GetCombatStartStrengthBonus()
    {
        var equippedQuartz = OrbmentManager.GetEquippedQuartz();

        var strengthBonus = 0;

        foreach (var quartz in equippedQuartz)
        {
            strengthBonus += quartz.Id switch
            {
                "attack_1" => 1,
                "attack_2" => 2,
                "attack_3" => 3,
                _ => 0
            };
        }

        return strengthBonus;
    }

    private static int GetCombatStartDexterityBonus()
    {
        var equippedQuartz = OrbmentManager.GetEquippedQuartz();

        var dexterityBonus = 0;

        foreach (var quartz in equippedQuartz)
        {
            dexterityBonus += quartz.Id switch
            {
                "defense_1" => 1,
                "defense_2" => 2,
                "defense_3" => 3,
                _ => 0
            };
        }

        return dexterityBonus;
    }
}