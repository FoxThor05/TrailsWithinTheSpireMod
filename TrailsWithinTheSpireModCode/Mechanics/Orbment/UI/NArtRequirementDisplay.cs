#nullable enable
using Godot;
using System.Collections.Generic;
using System.Linq;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

public partial class NArtRequirementDisplay : Control
{
    private static readonly Element[] ElementOrder =
    {
        Element.Earth,
        Element.Water,
        Element.Fire,
        Element.Wind,
        Element.Time,
        Element.Space,
        Element.Mirage
    };

    private const float PanelWidth = 54f;
    private const float RowHeight = 32f;
    private const float IconSize = 24f;

    public void SetRequirements(IReadOnlyDictionary<Element, int> requirements)
    {
        foreach (var child in GetChildren())
            child.QueueFree();

        var visibleRequirements = ElementOrder
            .Where(element => requirements.TryGetValue(element, out var value) && value > 0)
            .Select(element => new KeyValuePair<Element, int>(element, requirements[element]))
            .ToList();

        if (visibleRequirements.Count == 0)
        {
            Visible = false;
            return;
        }

        Visible = true;

        var height = visibleRequirements.Count * RowHeight + 8f;

        CustomMinimumSize = new Vector2(PanelWidth, height);
        Size = new Vector2(PanelWidth, height);
        MouseFilter = MouseFilterEnum.Ignore;
        ZIndex = 500;

        var background = new ColorRect
        {
            Name = "Background",
            Color = new Color(0.35f, 0.35f, 0.35f, 0.92f),
            MouseFilter = MouseFilterEnum.Ignore
        };

        background.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(background);

        var rows = new VBoxContainer
        {
            Name = "Rows",
            MouseFilter = MouseFilterEnum.Ignore,
            CustomMinimumSize = new Vector2(PanelWidth, height)
        };

        rows.SetAnchorsPreset(LayoutPreset.FullRect);
        rows.AddThemeConstantOverride("separation", 0);
        AddChild(rows);

        foreach (var requirement in visibleRequirements)
        {
            rows.AddChild(CreateRow(requirement.Key, requirement.Value));
        }
    }

    private Control CreateRow(Element element, int value)
    {
        var row = new HBoxContainer
        {
            Name = $"{element}Requirement",
            CustomMinimumSize = new Vector2(PanelWidth, RowHeight),
            MouseFilter = MouseFilterEnum.Ignore
        };

        row.AddThemeConstantOverride("separation", 2);

        var icon = new TextureRect
        {
            Name = "Icon",
            CustomMinimumSize = new Vector2(IconSize, IconSize),
            Size = new Vector2(IconSize, IconSize),
            MouseFilter = MouseFilterEnum.Ignore,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
        };

        var texture = GD.Load<Texture2D>(GetIconPathForElement(element));

        if (texture == null)
            GD.PrintErr($"NArtRequirementDisplay: Could not load icon for {element}.");
        else
            icon.Texture = texture;

        var label = new Label
        {
            Name = "Value",
            Text = value.ToString(),
            CustomMinimumSize = new Vector2(24, RowHeight),
            MouseFilter = MouseFilterEnum.Ignore,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        };

        label.AddThemeFontSizeOverride("font_size", 24);
        label.AddThemeColorOverride("font_color", Colors.White);
        label.AddThemeConstantOverride("outline_size", 5);
        label.AddThemeColorOverride("font_outline_color", Colors.Black);

        row.AddChild(icon);
        row.AddChild(label);

        return row;
    }

    private static string GetIconPathForElement(Element element)
    {
        return element switch
        {
            Element.Earth => "res://TrailsWithinTheSpireMod/assets/elements/earthquartz.png",
            Element.Water => "res://TrailsWithinTheSpireMod/assets/elements/waterquartz.png",
            Element.Fire => "res://TrailsWithinTheSpireMod/assets/elements/firequartz.png",
            Element.Wind => "res://TrailsWithinTheSpireMod/assets/elements/windquartz.png",
            Element.Time => "res://TrailsWithinTheSpireMod/assets/elements/timequartz.png",
            Element.Space => "res://TrailsWithinTheSpireMod/assets/elements/spacequartz.png",
            Element.Mirage => "res://TrailsWithinTheSpireMod/assets/elements/miragequartz.png",
            _ => ""
        };
    }
}