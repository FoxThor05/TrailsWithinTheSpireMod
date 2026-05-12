using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.TopBar;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

[HarmonyPatch(typeof(NTopBarButton), "_Ready")]
public static class NTopBarMapButtonOrbmentPatch
{
    private const string OrbmentButtonName = "OrbmentTopBarButton";

    [HarmonyPostfix]
    public static void Postfix(object __instance)
    {
        if (__instance is not NTopBarMapButton mapButton)
            return;

        if (!GodotObject.IsInstanceValid(mapButton))
            return;

        Callable.From(() => InjectDeferred(mapButton)).CallDeferred();
    }

    private static void InjectDeferred(NTopBarMapButton mapButton)
    {
        if (!GodotObject.IsInstanceValid(mapButton))
            return;

        var parent = mapButton.GetParent();

        if (parent == null || !GodotObject.IsInstanceValid(parent))
        {
            GD.PrintErr("ORBMENT_LOG: Could not inject Orbment top bar button because map button parent is null.");
            return;
        }

        if (parent.GetNodeOrNull<NOrbmentButton>(OrbmentButtonName) != null)
            return;

        foreach (var child in parent.GetChildren())
        {
            if (child.Name.ToString() == OrbmentButtonName)
                return;
        }

        var orbmentButton = new NOrbmentButton
        {
            Name = OrbmentButtonName,
            CustomMinimumSize = mapButton.CustomMinimumSize,
            Size = mapButton.Size,
            MouseFilter = Control.MouseFilterEnum.Stop
        };

        parent.AddChild(orbmentButton);

        var targetIndex = mapButton.GetIndex() + 1;

        if (targetIndex >= 0 && targetIndex < parent.GetChildCount())
            parent.MoveChild(orbmentButton, targetIndex);

        GD.Print($"ORBMENT_LOG: Orbment button injected into top bar parent '{parent.Name}' at index {orbmentButton.GetIndex()}.");
    }
}