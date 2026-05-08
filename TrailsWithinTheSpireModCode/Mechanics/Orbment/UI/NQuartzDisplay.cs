#nullable enable
using Godot;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

public partial class NQuartzDisplay : TextureButton
{
	private QuartzDefinition? _quartz;

	private TextureRect? _icon;
	private Label? _countLabel;

	public int Count { get; set; } = 1;

	public override void _Ready()
	{
		_icon = GetNodeOrNull<TextureRect>("%Icon");
		_countLabel = GetNodeOrNull<Label>("%CountLabel");

		MouseEntered += OnHovered;
		MouseExited += OnUnhovered;

		Reload();
	}

	public void SetQuartz(QuartzDefinition quartz, int count = 1)
	{
		_quartz = quartz;
		Count = count;

		if (IsNodeReady())
			Reload();
	}

	private void Reload()
	{
		if (_quartz == null)
			return;

		if (_countLabel != null)
			_countLabel.Text = Count > 1 ? Count.ToString() : "";

		// Later: set icon texture from quartz.IconPath or quartz.Element
		// For now, the scene can already have a placeholder texture assigned.
	}

	private void OnHovered()
	{
		if (_quartz == null)
			return;

		GD.Print($"Quartz hovered: {_quartz.Id}");
		// Next step: replace this with real hover tooltip display.
	}

	private void OnUnhovered()
	{
		if (_quartz == null)
			return;

		GD.Print($"Quartz unhovered: {_quartz.Id}");
		// Next step: hide tooltip.
	}
}
