using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Cards;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Character;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Cards.Arts;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;
[Pool(typeof(TrailsWithinTheSpireModCardPool))]
public sealed class FireBolt : TrailsWithinTheSpireModCard, IArtCard
{
    public string ArtId => "fire_bolt";

    public FireBolt()
        : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8M, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var fireVfx = NGroundFireVfx.Create(cardPlay.Target);
        if (fireVfx != null)
            NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(fireVfx);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust,
        CardKeyword.Ethereal
    ];
    
    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(3M);

}