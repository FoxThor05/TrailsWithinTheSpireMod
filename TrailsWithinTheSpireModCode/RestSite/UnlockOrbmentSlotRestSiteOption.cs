using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization;
using System.Threading.Tasks;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Relics;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.RestSite;

public sealed class UnlockOrbmentSlotRestSiteOption : RestSiteOption
{
    private readonly BattleOrbment _battleOrbment;

    public override string OptionId => "UNLOCK_ORBMENT_SLOT";

    public override LocString Description =>
        IsEnabled
            ? new LocString("rest_site_ui", $"OPTION_{OptionId}.description")
            : new LocString("rest_site_ui", $"OPTION_{OptionId}.descriptionDisabled");

    public UnlockOrbmentSlotRestSiteOption(Player owner, BattleOrbment battleOrbment) : base(owner)
    {
        _battleOrbment = battleOrbment;

        OrbmentRelicFields.Normalize(_battleOrbment);

        IsEnabled = OrbmentRelicFields.UnlockedSlots[_battleOrbment] < BattleOrbmentState.MaxSlots;
    }

    public override Task<bool> OnSelect()
    {
        if (!IsEnabled)
            return Task.FromResult(false);

        OrbmentManager.RegisterBattleOrbment(_battleOrbment);

        OrbmentManager.Current.UnlockSlot();
        OrbmentManager.NotifyOrbmentChanged();

        _battleOrbment.Flash();

        return Task.FromResult(true);
    }
}