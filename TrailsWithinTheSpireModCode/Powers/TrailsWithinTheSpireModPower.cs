using BaseLib.Abstracts;
using BaseLib.Extensions;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Extensions;
using Godot;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Powers;

public abstract class TrailsWithinTheSpireModPower : CustomPowerModel
{
    //Loads from TrailsWithinTheSpireMod/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}