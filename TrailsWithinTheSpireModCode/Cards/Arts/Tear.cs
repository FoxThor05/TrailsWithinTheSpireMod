using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Cards;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Character;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Cards.Arts;
[Pool(typeof(TrailsWithinTheSpireModCardPool))]
public sealed class Tear : TrailsWithinTheSpireModCard, IArtCard
{
    public string ArtId => "tear";
    public Tear()
        : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(3M)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust,
        CardKeyword.Ethereal
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }
    protected override void OnUpgrade() => this.DynamicVars.Heal.UpgradeValueBy(2M);

}