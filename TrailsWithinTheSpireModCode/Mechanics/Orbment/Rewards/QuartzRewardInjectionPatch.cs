using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rewards;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.Rewards;

[HarmonyPatch(typeof(RewardsSet), "GenerateWithoutOffering")]
public static class QuartzRewardInjectionPatch
{
    [HarmonyPostfix]
    public static void Postfix(RewardsSet __instance)
    {
        try
        {
            QuartzRewardRegistry.Register();

            var rewards = __instance.Rewards;
            var player = __instance.Player;
            var room = __instance.Room;

            if (player == null || room == null || rewards == null)
                return;

            if (rewards.Any(r => r is QuartzReward))
                return;

            var quartz = RollQuartzForRoom(player, room);

            if (quartz == null)
                return;

            rewards.Add(new QuartzReward(player, quartz.Id));

            GD.Print($"QUARTZ_REWARD_LOG: Added Quartz reward '{quartz.Id}' for room type '{GetRoomTypeName(room)}'.");
        }
        catch (Exception e)
        {
            GD.PrintErr($"QUARTZ_REWARD_LOG: Failed to inject Quartz reward: {e}");
        }
    }

    private static QuartzDefinition? RollQuartzForRoom(Player player, object room)
    {
        var roomTypeName = GetRoomTypeName(room);

        switch (roomTypeName)
        {
            case "Monster":
            case "Normal":
            case "Hallway":
                return RollChance(player, 0.50f)
                    ? PickRandomQuartzByTierWithFallback(player, 1)
                    : null;

            case "Elite":
                return RollChance(player, 0.75f)
                    ? PickRandomQuartzByTierWithFallback(player, 2)
                    : PickRandomQuartzByTierWithFallback(player, 1);

            case "Boss":
                return PickRandomQuartzByTierWithFallback(player, 3);

            default:
                return null;
        }
    }

    private static QuartzDefinition? PickRandomQuartzByTierWithFallback(Player player, int requestedTier)
    {
        for (var tier = requestedTier; tier >= 1; tier--)
        {
            var pool = QuartzDatabase.All
                .Where(q => q.Tier == tier)
                .ToList();

            if (pool.Count <= 0)
                continue;

            var index = RollIndex(player, pool.Count);
            return pool[index];
        }

        if (QuartzDatabase.All.Count <= 0)
            return null;

        return QuartzDatabase.All[RollIndex(player, QuartzDatabase.All.Count)];
    }

    private static bool RollChance(Player player, float chance)
    {
        return RollFloat(player) < chance;
    }

    private static int RollIndex(Player player, int count)
    {
        if (count <= 1)
            return 0;

        var roll = RollFloat(player);
        var index = Mathf.FloorToInt(roll * count);

        return Mathf.Clamp(index, 0, count - 1);
    }

    private static float RollFloat(Player player)
    {
        try
        {
            var playerRngProperty = player.GetType().GetProperty("PlayerRng");
            var playerRng = playerRngProperty?.GetValue(player);

            var rewardsRngProperty = playerRng?.GetType().GetProperty("Rewards");
            var rewardsRng = rewardsRngProperty?.GetValue(playerRng);

            if (rewardsRng != null)
            {
                var nextFloatMethod =
                    rewardsRng.GetType().GetMethod("NextFloat", Type.EmptyTypes) ??
                    rewardsRng.GetType().GetMethod("NextSingle", Type.EmptyTypes);

                if (nextFloatMethod != null)
                {
                    var value = nextFloatMethod.Invoke(rewardsRng, null);

                    if (value is float f)
                        return Mathf.Clamp(f, 0f, 0.999999f);

                    if (value is double d)
                        return Mathf.Clamp((float)d, 0f, 0.999999f);
                }
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"QUARTZ_REWARD_LOG: Failed to use player reward RNG, falling back to GD.Randf(): {e.Message}");
        }

        return Mathf.Clamp(GD.Randf(), 0f, 0.999999f);
    }

    private static string GetRoomTypeName(object room)
    {
        var property = room.GetType().GetProperty("RoomType");

        if (property == null)
            return "";

        var value = property.GetValue(room);

        return value?.ToString() ?? "";
    }
}