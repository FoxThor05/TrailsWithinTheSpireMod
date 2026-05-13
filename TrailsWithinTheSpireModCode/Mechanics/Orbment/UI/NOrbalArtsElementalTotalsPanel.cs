#nullable enable
using Godot;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

public static class NOrbalArtsElementalTotalsPanel
{
    public const string NodeName = "OrbalArtsElementalTotalsPanel";

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

    private const float PanelWidth = 170f;
    private const float PanelHeight = 360f;
    private const float RowHeight = 44f;
    private const float IconSize = 36f;

    public static Control Create()
    {
        var panel = new Control
        {
            Name = NodeName,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            CustomMinimumSize = new Vector2(PanelWidth, PanelHeight),
            Size = new Vector2(PanelWidth, PanelHeight),
            ZIndex = 900
        };

        BuildUi(panel);
        Refresh(panel);

        return panel;
    }

    public static void Refresh(Control panel)
    {
        var rows = panel.GetNodeOrNull<VBoxContainer>("Rows");

        if (rows == null)
            return;

        foreach (var child in rows.GetChildren())
        {
            rows.RemoveChild(child);
            child.QueueFree();
        }

        var totals = OrbmentManager.Current.GetElementTotals();

        foreach (var element in ElementOrder)
        {
            totals.TryGetValue(element, out var value);
            rows.AddChild(CreateRow(element, value));
        }
    }

    private static void BuildUi(Control panel)
    {
        foreach (var child in panel.GetChildren())
        {
            panel.RemoveChild(child);
            child.QueueFree();
        }

        var background = new ColorRect
        {
            Name = "Background",
            Color = new Color(0.08f, 0.1f, 0.11f, 0.78f),
            MouseFilter = Control.MouseFilterEnum.Ignore
        };

        background.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        panel.AddChild(background);

        var border = new Panel
        {
            Name = "Border",
            MouseFilter = Control.MouseFilterEnum.Ignore
        };

        border.SetAnchorsPreset(Control.LayoutPreset.FullRect);

        var borderStyle = new StyleBoxFlat
        {
            DrawCenter = false,
            BorderWidthLeft = 3,
            BorderWidthTop = 3,
            BorderWidthRight = 3,
            BorderWidthBottom = 3,
            BorderColor = new Color(0.35f, 0.55f, 0.62f, 0.95f)
        };

        border.AddThemeStyleboxOverride("panel", borderStyle);
        panel.AddChild(border);

        var title = new Label
        {
            Name = "Title",
            Text = "Elemental Values",
            CustomMinimumSize = new Vector2(PanelWidth, 38f),
            Size = new Vector2(PanelWidth, 38f),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };

        title.AddThemeFontSizeOverride("font_size", 17);
        title.AddThemeColorOverride("font_color", Colors.White);
        title.AddThemeConstantOverride("outline_size", 5);
        title.AddThemeColorOverride("font_outline_color", Colors.Black);

        panel.AddChild(title);

        var rows = new VBoxContainer
        {
            Name = "Rows",
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Position = new Vector2(22f, 46f),
            CustomMinimumSize = new Vector2(PanelWidth - 44f, PanelHeight - 58f),
            Size = new Vector2(PanelWidth - 44f, PanelHeight - 58f)
        };

        rows.AddThemeConstantOverride("separation", 0);
        panel.AddChild(rows);
    }

    private static Control CreateRow(Element element, int value)
    {
        var row = new HBoxContainer
        {
            Name = $"{element}Row",
            CustomMinimumSize = new Vector2(PanelWidth - 36f, RowHeight),
            Size = new Vector2(PanelWidth - 36f, RowHeight),
            MouseFilter = Control.MouseFilterEnum.Ignore
        };

        row.AddThemeConstantOverride("separation", 10);

        if (value <= 0)
            row.Modulate = new Color(1f, 1f, 1f, 0.45f);

        var icon = new TextureRect
        {
            Name = "Icon",
            CustomMinimumSize = new Vector2(IconSize, IconSize),
            Size = new Vector2(IconSize, IconSize),
            MouseFilter = Control.MouseFilterEnum.Ignore,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
        };

        var texture = GD.Load<Texture2D>(GetIconPathForElement(element));

        if (texture == null)
            GD.PrintErr($"NOrbalArtsElementalTotalsPanel: Could not load icon for {element}.");
        else
            icon.Texture = texture;

        var label = new Label
        {
            Name = "Value",
            Text = value.ToString(),
            CustomMinimumSize = new Vector2(56f, RowHeight),
            Size = new Vector2(56f, RowHeight),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };

        label.AddThemeFontSizeOverride("font_size", 28);
        label.AddThemeColorOverride("font_color", Colors.White);
        label.AddThemeConstantOverride("outline_size", 6);
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