using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Cards;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Character;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Cards.Arts;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;
[Pool(typeof(TrailsWithinTheSpireModCardPool))]
public sealed class Crest : TrailsWithinTheSpireModCard, IArtCard
{
    public string ArtId => "crest";

    public Crest()
        : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
    }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(9M, ValueProp.Move)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust,
        CardKeyword.Ethereal
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }
    protected override void OnUpgrade() => this.DynamicVars.Block.UpgradeValueBy(3M); // Corrected to upgrade BlockVar
}