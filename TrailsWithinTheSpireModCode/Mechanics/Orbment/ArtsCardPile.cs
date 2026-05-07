using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public class ArtsCardPile() : CustomPile(ArtsPileType)
{
    [CustomEnum] public static PileType ArtsPileType;

    public override bool CardShouldBeVisible(CardModel card)
    {
        return true;
    }

    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        return new Vector2(0, 0); 
    }
}