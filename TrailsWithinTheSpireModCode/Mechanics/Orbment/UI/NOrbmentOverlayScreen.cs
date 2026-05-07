using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

public partial class NOrbmentOverlayScreen : Control, IOverlayScreen
{
    public NetScreenType ScreenType => NetScreenType.None;
    public bool UseSharedBackstop => true;

    public void AfterOverlayOpened()
    {
        GD.Print("NOrbmentOverlayScreen: AfterOverlayOpened");
        this.Visible = true;
    }

    public void AfterOverlayClosed()
    {
        GD.Print("NOrbmentOverlayScreen: AfterOverlayClosed");
        this.Visible = false;
    }

    public void AfterOverlayShown()
    {
        GD.Print("NOrbmentOverlayScreen: AfterOverlayShown");
    }

    public void AfterOverlayHidden()
    {
        GD.Print("NOrbmentOverlayScreen: AfterOverlayHidden");
    }
    
    public Control GetControl() => this;
    public bool IsCurrent(IScreenContext context) => context == this;
    public void Update() { }
    public Control? DefaultFocusedControl => null; 


    private Player? _currentPlayer;

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

        this.FocusMode = Control.FocusModeEnum.All;
        this.FocusBehaviorRecursive = FocusBehaviorRecursiveEnum.Enabled;
        this.MouseFilter = MouseFilterEnum.Stop;

        GD.Print("NOrbmentOverlayScreen: _Ready finished.");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            GD.Print("NOrbmentOverlayScreen: Esc detected. Removing from OverlayStack.");
            NOverlayStack.Instance?.Remove(this);
            GetViewport().SetInputAsHandled();
        }
    }

    public override void _ExitTree()
    {
        GD.Print("NOrbmentOverlayScreen: _ExitTree called.");
        base._ExitTree();
    }
}
