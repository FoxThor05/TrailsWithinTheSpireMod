using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

[HarmonyPatch(typeof(NCardPileScreen), "_Ready")]
public static class NCardPileScreenPatch
{
    public static bool IsCastingModeActive = false;
    private static NCardPileScreen? _currentArtsPileScreen;

    [HarmonyPostfix]
    public static void Postfix(NCardPileScreen __instance)
    {
        var pile = __instance.Pile;
        if (pile == null || pile.Type != ArtsCardPile.ArtsPileType) return;

    }

    [HarmonyPatch("_ExitTree")]
    [HarmonyPostfix]
    public static void Postfix_ExitTree(NCardPileScreen __instance)
    {
        if (__instance.Pile.Type != ArtsCardPile.ArtsPileType) return;

        IsCastingModeActive = false;
        _currentArtsPileScreen = null;
    }
}
