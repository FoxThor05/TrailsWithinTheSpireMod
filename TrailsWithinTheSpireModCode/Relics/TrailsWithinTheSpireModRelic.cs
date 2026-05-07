using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Character;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Extensions;
using Godot;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Relics;

[Pool(typeof(TrailsWithinTheSpireModRelicPool))]
public abstract class TrailsWithinTheSpireModRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();

    protected override string PackedIconOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();

    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}