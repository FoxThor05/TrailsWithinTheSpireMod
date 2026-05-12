#nullable enable
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;
using System.Collections.Generic;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

[HarmonyPatch(typeof(NCard), nameof(NCard.UpdateVisuals))]
public static class NCardArtRequirementDisplayPatch
{
    private const string RequirementDisplayNodeName = "ArtRequirementDisplay";

    private const float PortraitRightOverlap = 8f;
    private const float PortraitTopOffset = 4f;

    [HarmonyPostfix]
    public static void Postfix(NCard __instance)
    {
        RefreshRequirementDisplay(__instance);
    }

    private static void RefreshRequirementDisplay(NCard card)
    {
        if (!GodotObject.IsInstanceValid(card))
            return;

        if (!card.IsNodeReady())
            return;

        RemoveAllRequirementDisplays(card);

        if (card.Model is not IArtCard artCard)
            return;

        var shouldShow =
            card.DisplayingPile == ArtsCardPile.ArtsPileType ||
            NOrbalArtsSelectionRegistry.IsOrbalArtsCard(card.Model);

        if (!shouldShow)
            return;

        var artDefinition = ArtDatabase.GetById(artCard.ArtId);

        if (artDefinition == null)
        {
            GD.PrintErr($"NCardArtRequirementDisplayPatch: No ArtDefinition found for ArtId '{artCard.ArtId}'.");
            return;
        }

        if (artDefinition.Requirements.Count == 0)
            return;

        if (card.Body == null)
        {
            GD.PrintErr("NCardArtRequirementDisplayPatch: NCard.Body was null.");
            return;
        }

        if (NOrbalArtsSelectionRegistry.IsOrbalArtsCard(card.Model))
        {
            card.Modulate = NOrbalArtsSelectionRegistry.IsDisabled(card.Model)
                ? new Color(0.45f, 0.45f, 0.45f, 0.65f)
                : Colors.White;
        }
        else
        {
            card.Modulate = Colors.White;
        }

        var display = new NArtRequirementDisplay
        {
            Name = RequirementDisplayNodeName,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            ZIndex = 500
        };

        card.Body.AddChild(display);
        display.SetRequirements(artDefinition.Requirements);

        var portrait = card.GetNodeOrNull<Control>("%Portrait");

        if (portrait == null || !TryGetLocalPositionRelativeToAncestor(portrait, card.Body, out var portraitLocalPosition))
        {
            display.Position = new Vector2(238f, 70f);
            return;
        }

        display.Position = new Vector2(
            portraitLocalPosition.X + portrait.Size.X - PortraitRightOverlap,
            portraitLocalPosition.Y + PortraitTopOffset
        );
    }

    private static bool TryGetLocalPositionRelativeToAncestor(
        Control node,
        Control ancestor,
        out Vector2 localPosition)
    {
        localPosition = node.Position;

        var current = node.GetParent();

        while (current != null)
        {
            if (current == ancestor)
                return true;

            if (current is Control currentControl)
                localPosition += currentControl.Position;

            current = current.GetParent();
        }

        localPosition = Vector2.Zero;
        return false;
    }

    private static void RemoveAllRequirementDisplays(Node root)
    {
        var nodesToRemove = new List<Node>();
        CollectRequirementDisplays(root, nodesToRemove);

        foreach (var node in nodesToRemove)
        {
            if (!GodotObject.IsInstanceValid(node))
                continue;

            node.GetParent()?.RemoveChild(node);
            node.QueueFree();
        }
    }

    private static void CollectRequirementDisplays(Node node, List<Node> results)
    {
        foreach (var child in node.GetChildren())
        {
            if (child.Name.ToString() == RequirementDisplayNodeName)
                results.Add(child);

            CollectRequirementDisplays(child, results);
        }
    }
}