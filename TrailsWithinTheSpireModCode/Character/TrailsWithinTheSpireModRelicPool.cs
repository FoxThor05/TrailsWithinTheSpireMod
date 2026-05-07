using BaseLib.Abstracts;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Extensions;
using Godot;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Character;

public class TrailsWithinTheSpireModRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => TrailsWithinTheSpireMod.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}