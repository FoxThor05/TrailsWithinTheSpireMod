using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public static class OrbmentRelicFields
{
    public const string EmptySlotsEncoded = "||||||";

    public static readonly SavedSpireField<RelicModel, int> UnlockedSlots =
        new(() => 1, "TRAILS_ORBMENT_UNLOCKED_SLOTS");

    public static readonly SavedSpireField<RelicModel, string> OwnedQuartz =
        new(() => "", "TRAILS_ORBMENT_OWNED_QUARTZ");

    public static readonly SavedSpireField<RelicModel, string> EquippedQuartz =
        new(() => EmptySlotsEncoded, "TRAILS_ORBMENT_EQUIPPED_QUARTZ");
}