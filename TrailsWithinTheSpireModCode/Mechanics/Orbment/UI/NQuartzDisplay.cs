#nullable enable
using Godot;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

public partial class NQuartzDisplay : NClickableControl
{
	private const string QuartzLocTable = "cards";

	private QuartzDefinition? _quartz;

	private TextureRect? _icon;
	private Label? _countLabel;

	public int Count { get; private set; } = 1;

	public override void _Ready()
	{
		CustomMinimumSize = new Vector2(64, 64);
		MouseFilter = MouseFilterEnum.Stop;

		_icon = GetNodeOrNull<TextureRect>("%Icon");
		_countLabel = GetNodeOrNull<Label>("%CountLabel");

		if (_icon != null)
			_icon.MouseFilter = MouseFilterEnum.Ignore;

		if (_countLabel != null)
			_countLabel.MouseFilter = MouseFilterEnum.Ignore;

		MouseEntered += OnHovered;
		MouseExited += OnUnhovered;

		Reload();
	}

	public override void _ExitTree()
	{
		NHoverTipSet.Remove(this);

		MouseEntered -= OnHovered;
		MouseExited -= OnUnhovered;

		base._ExitTree();
	}

	public void SetQuartz(QuartzDefinition quartz, int count = 1)
	{
		_quartz = quartz;
		Count = count;

		GD.Print($"NQuartzDisplay: SetQuartz {quartz.Id} using loc table '{QuartzLocTable}'.");

		if (IsNodeReady())
			Reload();
	}

	private void Reload()
	{
		if (_quartz == null)
			return;

		if (_countLabel != null)
			_countLabel.Text = Count > 1 ? Count.ToString() : "";

		if (_icon == null)
		{
			GD.PrintErr("NQuartzDisplay: Icon node not found.");
			return;
		}

		if (string.IsNullOrWhiteSpace(_quartz.IconPath))
		{
			GD.PrintErr($"NQuartzDisplay: Quartz '{_quartz.Id}' has no IconPath.");
			return;
		}

		var texture = GD.Load<Texture2D>(_quartz.IconPath);

		if (texture == null)
		{
			GD.PrintErr($"NQuartzDisplay: Could not load texture at '{_quartz.IconPath}' for quartz '{_quartz.Id}'.");
			return;
		}

		_icon.Texture = texture;
	}

	private void OnHovered()
	{
		if (_quartz == null)
			return;

		try
		{
			var hoverTip = new HoverTip(
				new LocString(QuartzLocTable, $"{_quartz.Id}.title"),
				new LocString(QuartzLocTable, $"{_quartz.Id}.description")
			);

			NHoverTipSet.CreateAndShow(this, hoverTip)
				?.SetGlobalPosition(GlobalPosition + new Vector2(70, 0));
		}
		catch (System.Exception ex)
		{
			GD.PrintErr($"NQuartzDisplay: Failed to show tooltip for '{_quartz.Id}': {ex.Message}");
		}
	}

	private void OnUnhovered()
	{
		NHoverTipSet.Remove(this);
	}
}
