#nullable enable
using Godot;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

public partial class NElementalValueDisplay : NClickableControl
{
    private TextureRect? _icon;
    private Label? _valueLabel;

    private Element _element;
    private bool _hasElement;

    public override void _Ready()
    {
        base._Ready();

        CustomMinimumSize = new Vector2(100, 40);
        MouseFilter = MouseFilterEnum.Ignore;

        _icon = GetNodeOrNull<TextureRect>("%Icon");
        _valueLabel = GetNodeOrNull<Label>("%ValueLabel");

        if (_icon != null)
            _icon.MouseFilter = MouseFilterEnum.Ignore;

        if (_valueLabel != null)
            _valueLabel.MouseFilter = MouseFilterEnum.Ignore;

        _hasElement = TryGetElementFromNodeName(out _element);

        if (!_hasElement)
        {
            GD.PrintErr($"NElementalValueDisplay: Could not determine element from node name '{Name}'. Rename it to Earth, Water, Fire, Wind, Time, Space, or Mirage.");
            return;
        }

        LoadIcon();
        Refresh();
    }

    public void Refresh()
    {
        if (!_hasElement)
            return;

        if (_valueLabel == null)
            return;

        var totals = OrbmentManager.Current.GetElementTotals();

        if (!totals.TryGetValue(_element, out var value))
            value = 0;

        _valueLabel.Text = value.ToString();
    }

    private void LoadIcon()
    {
        if (_icon == null)
        {
            GD.PrintErr($"NElementalValueDisplay: Icon node not found for '{Name}'.");
            return;
        }

        var iconPath = GetIconPathForElement(_element);

        if (string.IsNullOrWhiteSpace(iconPath))
        {
            GD.PrintErr($"NElementalValueDisplay: No icon path set for element '{_element}'.");
            return;
        }

        var texture = GD.Load<Texture2D>(iconPath);

        if (texture == null)
        {
            GD.PrintErr($"NElementalValueDisplay: Could not load icon for '{_element}' at path '{iconPath}'.");
            return;
        }

        _icon.Texture = texture;
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

    private bool TryGetElementFromNodeName(out Element element)
    {
        var nodeName = Name.ToString().ToLowerInvariant();

        if (nodeName.Contains("earth"))
        {
            element = Element.Earth;
            return true;
        }

        if (nodeName.Contains("water"))
        {
            element = Element.Water;
            return true;
        }

        if (nodeName.Contains("fire"))
        {
            element = Element.Fire;
            return true;
        }

        if (nodeName.Contains("wind"))
        {
            element = Element.Wind;
            return true;
        }

        if (nodeName.Contains("time"))
        {
            element = Element.Time;
            return true;
        }

        if (nodeName.Contains("space"))
        {
            element = Element.Space;
            return true;
        }

        if (nodeName.Contains("mirage"))
        {
            element = Element.Mirage;
            return true;
        }

        element = default;
        return false;
    }
}