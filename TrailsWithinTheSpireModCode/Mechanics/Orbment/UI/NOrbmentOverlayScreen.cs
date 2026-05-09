using Godot;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

public partial class NOrbmentOverlayScreen : NClickableControl, IOverlayScreen
{
	public NetScreenType ScreenType => NetScreenType.None;
	public bool UseSharedBackstop => true;

	private Player? _currentPlayer;

	public Control GetControl() => this;
	public bool IsCurrent(IScreenContext context) => context == this;
	public void Update() { }
	public Control? DefaultFocusedControl => null;

	public NOrbmentOverlayScreen()
	{
		GD.Print("NOrbmentOverlayScreen: Constructor called.");
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		GD.Print("NOrbmentOverlayScreen: _EnterTree called.");
	}

	public override void _Ready()
	{
		GD.Print("NOrbmentOverlayScreen: _Ready started.");

		FocusMode = FocusModeEnum.All;
		FocusBehaviorRecursive = FocusBehaviorRecursiveEnum.Enabled;
		MouseFilter = MouseFilterEnum.Stop;

		var exitButton = GetNodeOrNull<Button>("%ExitButton");
		if (exitButton != null)
		{
			exitButton.MouseFilter = MouseFilterEnum.Stop;
			exitButton.Pressed += OnExitButtonPressed;
			GD.Print("NOrbmentOverlayScreen: ExitButton connected.");
		}
		else
		{
			GD.Print("NOrbmentOverlayScreen: ExitButton not found.");
		}

		var quartzContainer = GetNodeOrNull<GridContainer>("%OwnedQuartzListContainer");
		if (quartzContainer == null)
		{
			GD.PrintErr("NOrbmentOverlayScreen: OwnedQuartzListContainer not found or is not a GridContainer.");
			return;
		}

		quartzContainer.Columns = 4;

		foreach (var child in quartzContainer.GetChildren())
		{
			child.QueueFree();
		}

		var quartzScene = GD.Load<PackedScene>("res://TrailsWithinTheSpireMod/scenes/QuartzDisplay.tscn");
		if (quartzScene == null)
		{
			GD.PrintErr("NOrbmentOverlayScreen: Could not load QuartzDisplay.tscn.");
			return;
		}

		GD.Print($"NOrbmentOverlayScreen: Owned Quartz count: {OrbmentManager.OwnedQuartzIds.Count}");

		var quartzCounts = new Dictionary<string, int>();

		foreach (var quartzId in OrbmentManager.OwnedQuartzIds)
		{
			if (!quartzCounts.ContainsKey(quartzId))
				quartzCounts[quartzId] = 0;

			quartzCounts[quartzId]++;
		}

		foreach (var pair in quartzCounts)
		{
			var quartz = QuartzDatabase.GetById(pair.Key);

			if (quartz == null)
			{
				GD.PrintErr($"NOrbmentOverlayScreen: Unknown quartz id in inventory: {pair.Key}");
				continue;
			}

			var display = quartzScene.Instantiate<NQuartzDisplay>();
			display.SetQuartz(quartz, pair.Value);
			quartzContainer.AddChild(display);

			GD.Print($"NOrbmentOverlayScreen: Added QuartzDisplay for {pair.Key} x{pair.Value}.");
		}

		GD.Print("NOrbmentOverlayScreen: _Ready finished.");
	}

	private void OnExitButtonPressed()
	{
		GD.Print("NOrbmentOverlayScreen: ExitButton pressed.");
		NOverlayStack.Instance?.Remove(this);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))
		{
			GD.Print("NOrbmentOverlayScreen: Esc detected.");
			NOverlayStack.Instance?.Remove(this);
			GetViewport().SetInputAsHandled();
		}
	}

	public override void _ExitTree()
	{
		GD.Print("NOrbmentOverlayScreen: _ExitTree called.");
		base._ExitTree();
	}

	public void AfterOverlayOpened()
	{
		GD.Print("NOrbmentOverlayScreen: AfterOverlayOpened");
		Visible = true;
	}

	public void AfterOverlayClosed()
	{
		GD.Print("NOrbmentOverlayScreen: AfterOverlayClosed");
		Visible = false;
	}

	public void AfterOverlayShown()
	{
		GD.Print("NOrbmentOverlayScreen: AfterOverlayShown");
	}

	public void AfterOverlayHidden()
	{
		GD.Print("NOrbmentOverlayScreen: AfterOverlayHidden");
	}
}
