using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Rewards;
using System;
using System.Reflection;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.Rewards;

public static class QuartzRewardRegistry
{
    private static bool _registered;

    public static void Register()
    {
        if (_registered)
            return;

        try
        {
            var customRewardPatchesType =
                Type.GetType("Baselib.Patches.Content.CustomRewardPatches, BaseLib") ??
                Type.GetType("BaseLib.Patches.Content.CustomRewardPatches, BaseLib");

            if (customRewardPatchesType == null)
            {
                GD.PrintErr("QUARTZ_REWARD_LOG: Could not find BaseLib CustomRewardPatches type.");
                return;
            }

            var registerMethod = customRewardPatchesType.GetMethod(
                "RegisterCustomReward",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static
            );

            if (registerMethod == null)
            {
                GD.PrintErr("QUARTZ_REWARD_LOG: Could not find CustomRewardPatches.RegisterCustomReward.");
                return;
            }

            CreateRewardFromSave<CustomReward> deserializer = QuartzReward.CreateFromSerializable;

            registerMethod.Invoke(
                null,
                new object[]
                {
                    QuartzReward.QuartzRewardType,
                    deserializer
                }
            );

            _registered = true;

            GD.Print($"QUARTZ_REWARD_LOG: Registered QuartzReward with RewardType {(int)QuartzReward.QuartzRewardType}.");
        }
        catch (Exception e)
        {
            GD.PrintErr($"QUARTZ_REWARD_LOG: Failed to register QuartzReward: {e}");
        }
    }
}