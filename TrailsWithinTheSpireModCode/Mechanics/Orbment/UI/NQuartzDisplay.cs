#nullable enable
using Godot;
using GodotDictionary = Godot.Collections.Dictionary;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

public partial class NQuartzDisplay : NClickableControl
{
	public const string DragKindQuartz = "quartz";
	public const string DragSourceInventory = "inventory";
	public const string DragSourceSlot = "slot";

	private const string QuartzLocTable = "cards";

	private QuartzDefinition? _quartz;

	private TextureRect? _icon;
	private Label? _countLabel;

	private Tween? _hoverTween;

	private string _dragSource = DragSourceInventory;
	private int _sourceSlotIndex = -1;

	public int Count { get; private set; } = 1;

	public override void _Ready()
	{
		base._Ready();

		if (CustomMinimumSize == Vector2.Zero)
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
		_hoverTween?.Kill();

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

	public void SetDragContextInventory()
	{
		_dragSource = DragSourceInventory;
		_sourceSlotIndex = -1;
	}

	public void SetDragContextSlot(int slotIndex)
	{
		_dragSource = DragSourceSlot;
		_sourceSlotIndex = slotIndex;
	}

	public override Variant _GetDragData(Vector2 atPosition)
	{
		if (_quartz == null)
			return default;

		var data = new GodotDictionary
		{
			["kind"] = DragKindQuartz,
			["quartzId"] = _quartz.Id,
			["source"] = _dragSource,
			["slotIndex"] = _sourceSlotIndex
		};

		var preview = CreateDragPreview();
		SetDragPreview(preview);

		return data;
	}

	private Control CreateDragPreview()
	{
		var preview = new TextureRect
		{
			CustomMinimumSize = new Vector2(54, 54),
			Size = new Vector2(54, 54),
			MouseFilter = MouseFilterEnum.Ignore,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
		};

		if (_icon != null)
			preview.Texture = _icon.Texture;

		return preview;
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
		if (_quartz != null)
		{
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

		AnimateHoverScale(1.12f);
	}

	private void OnUnhovered()
	{
		NHoverTipSet.Remove(this);
		AnimateHoverScale(1.0f);
	}

	private void AnimateHoverScale(float targetScale)
	{
		if (_icon == null)
			return;

		_hoverTween?.Kill();

		_icon.PivotOffset = _icon.Size / 2f;

		_hoverTween = CreateTween();
		_hoverTween.TweenProperty(
			_icon,
			"scale",
			Vector2.One * targetScale,
			0.08
		);
	}

	public static bool TryReadQuartzDragData(
		Variant data,
		out string quartzId,
		out string source,
		out int sourceSlotIndex)
	{
		quartzId = "";
		source = "";
		sourceSlotIndex = -1;

		if (data.VariantType != Variant.Type.Dictionary)
			return false;

		var dict = data.AsGodotDictionary();

		if (!dict.ContainsKey("kind") || dict["kind"].AsString() != DragKindQuartz)
			return false;

		if (!dict.ContainsKey("quartzId") || !dict.ContainsKey("source"))
			return false;

		quartzId = dict["quartzId"].AsString();
		source = dict["source"].AsString();

		if (dict.ContainsKey("slotIndex"))
			sourceSlotIndex = dict["slotIndex"].AsInt32();

		return !string.IsNullOrWhiteSpace(quartzId);
	}
}
