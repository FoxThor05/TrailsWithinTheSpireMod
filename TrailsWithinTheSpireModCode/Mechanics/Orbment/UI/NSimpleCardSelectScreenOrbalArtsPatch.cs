using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

[HarmonyPatch(typeof(NSimpleCardSelectScreen), "OnCardClicked")]
public static class NSimpleCardSelectScreenOrbalArtsClickPatch
{
    [HarmonyPrefix]
    public static bool Prefix(NSimpleCardSelectScreen __instance, CardModel card)
    {
        if (!NOrbalArtsSelectionRegistry.IsOrbalArtsScreen(__instance))
            return true;

        if (NOrbalArtsSelectionRegistry.CanSelect(card))
            return true;

        if (card is IArtCard artCard)
            GD.Print($"ARTS_LOG: Art '{artCard.ArtId}' cannot be selected right now.");

        return false;
    }
}

[HarmonyPatch(typeof(NSimpleCardSelectScreen), "_ExitTree")]
public static class NSimpleCardSelectScreenOrbalArtsExitPatch
{
    [HarmonyPostfix]
    public static void Postfix(NSimpleCardSelectScreen __instance)
    {
        NOrbalArtsSelectionRegistry.Unregister(__instance);
    }
}