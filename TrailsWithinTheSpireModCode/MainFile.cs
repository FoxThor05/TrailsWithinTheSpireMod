using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using System;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.Rewards; // For Exception
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI; // For ArtsCardPile

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "TrailsWithinTheSpireMod";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        // Use Harmony.PatchAll to automatically discover and apply all patches in this assembly
        try
        {
            QuartzRewardRegistry.Register();
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            GD.Print($"MainFile: Successfully applied Harmony patches via PatchAll.");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"MainFile: Error applying Harmony patches via PatchAll: {ex.Message}");
        }

        Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());
        _ = ArtsCardPile.ArtsPileType; // Ensure CustomEnum is initialized
        
        GD.Print($"{ModId}: Initialization Complete.");
    }
}