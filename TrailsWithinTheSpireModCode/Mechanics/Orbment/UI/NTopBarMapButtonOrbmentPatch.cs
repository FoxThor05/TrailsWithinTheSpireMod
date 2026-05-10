using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.TopBar;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

[HarmonyPatch(typeof(NTopBarButton), "_Ready")]
public static class NTopBarMapButtonOrbmentPatch
{
    [HarmonyPostfix]
    public static void Postfix(NTopBarButton __instance)
    {
        if (__instance is not NTopBarMapButton mapButton)
            return;

        GD.Print("ORBMENT_LOG: NTopBarButton._Ready patch fired for NTopBarMapButton.");

        Callable.From(() => InjectDeferred(mapButton)).CallDeferred();
    }

    private static void InjectDeferred(NTopBarMapButton mapButton)
    {
        if (!GodotObject.IsInstanceValid(mapButton))
        {
            GD.PrintErr("ORBMENT_LOG: Deferred top bar injection failed because map button is invalid.");
            return;
        }

        var parent = mapButton.GetParent();

        if (parent == null || !GodotObject.IsInstanceValid(parent))
        {
            GD.PrintErr("ORBMENT_LOG: Deferred top bar injection failed because parent is invalid.");
            return;
        }

        GD.Print($"ORBMENT_LOG: Deferred top bar injection. Parent is '{parent.Name}' type '{parent.GetType().FullName}'.");

        if (parent.GetNodeOrNull<NOrbmentButton>("NOrbmentButton") != null)
        {
            GD.Print("ORBMENT_LOG: NOrbmentButton already exists in top bar parent.");
            return;
        }

        var orbmentButton = NOrbmentButton.Create();
        orbmentButton.Name = "NOrbmentButton";
        orbmentButton.CustomMinimumSize = new Vector2(56, 56);
        orbmentButton.Size = new Vector2(56, 56);

        parent.AddChild(orbmentButton);

        var targetIndex = mapButton.GetIndex() + 1;

        if (orbmentButton.GetParent() == parent)
            parent.MoveChild(orbmentButton, targetIndex);

        orbmentButton.Initialize();

        GD.Print($"ORBMENT_LOG: Orbment button injected into top bar parent at index {targetIndex}.");
    }
}