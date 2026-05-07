using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Godot;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

[HarmonyPatch(typeof(NCombatUi), "Activate")]
public static class NArtsCombatUiActivatePatch
{
    [HarmonyPostfix]
    public static void ActivatePostfix(NCombatUi __instance, CombatState state)
    {
        var container = __instance.GetNodeOrNull<NCombatPilesContainer>("%CombatPileContainer");
        var artsOverlay = container?.GetNodeOrNull<NArtsPile>("_ArtsPile");
        var player = LocalContext.GetMe(state);
        artsOverlay?.Initialize(player);
    }
}