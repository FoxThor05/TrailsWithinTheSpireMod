using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.Relics;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Relics;


public sealed class BattleOrbment : RelicModel
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
}