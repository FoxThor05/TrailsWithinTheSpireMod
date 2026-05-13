using System.Collections.Generic;
using System.Linq;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public static class QuartzDatabase
{
    private const string Earth1Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier1_earth.png";
    private const string Earth2Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier2_earth.png";
    private const string Earth3Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier3_earth.png";

    private const string Water1Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier1_water.png";
    private const string Water2Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier2_water.png";
    private const string Water3Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier3_water.png";

    private const string Fire1Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier1_fire.png";
    private const string Fire2Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier2_fire.png";
    private const string Fire3Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier3_fire.png";

    private const string Wind1Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier1_wind.png";
    private const string Wind2Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier2_wind.png";
    private const string Wind3Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier3_wind.png";

    private const string Time1Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier1_time.png";
    private const string Time2Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier2_time.png";
    private const string Time3Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier3_time.png";

    private const string Space1Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier1_space.png";
    private const string Space2Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier2_space.png";
    private const string Space3Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier3_space.png";

    private const string Mirage1Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier1_mirage.png";
    private const string Mirage2Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier2_mirage.png";
    private const string Mirage3Icon = "res://TrailsWithinTheSpireMod/images/quartz/tier3_mirage.png";

    public static readonly QuartzDefinition Defense1 = new()
    {
        Id = "defense_1",
        RewardSaveId = 1001,
        Tier = 1,
        IconPath = Earth1Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Earth, 1 }
        }
    };

    public static readonly QuartzDefinition Defense2 = new()
    {
        Id = "defense_2",
        RewardSaveId = 1002,
        Tier = 2,
        IconPath = Earth2Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Earth, 3 }
        }
    };

    public static readonly QuartzDefinition Defense3 = new()
    {
        Id = "defense_3",
        RewardSaveId = 1003,
        Tier = 3,
        IconPath = Earth3Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Earth, 5 }
        }
    };

    public static readonly QuartzDefinition Hp1 = new()
    {
        Id = "hp_1",
        RewardSaveId = 1101,
        Tier = 1,
        IconPath = Water1Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Water, 1 }
        }
    };

    public static readonly QuartzDefinition Hp2 = new()
    {
        Id = "hp_2",
        RewardSaveId = 1102,
        Tier = 2,
        IconPath = Water2Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Water, 3 }
        }
    };

    public static readonly QuartzDefinition Hp3 = new()
    {
        Id = "hp_3",
        RewardSaveId = 1103,
        Tier = 3,
        IconPath = Water3Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Water, 5 }
        }
    };

    public static readonly QuartzDefinition Attack1 = new()
    {
        Id = "attack_1",
        RewardSaveId = 1201,
        Tier = 1,
        IconPath = Fire1Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Fire, 1 }
        }
    };

    public static readonly QuartzDefinition Attack2 = new()
    {
        Id = "attack_2",
        RewardSaveId = 1202,
        Tier = 2,
        IconPath = Fire2Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Fire, 3 }
        }
    };

    public static readonly QuartzDefinition Attack3 = new()
    {
        Id = "attack_3",
        RewardSaveId = 1203,
        Tier = 3,
        IconPath = Fire3Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Fire, 5 }
        }
    };

    public static readonly QuartzDefinition Evade1 = new()
    {
        Id = "evade_1",
        RewardSaveId = 1301,
        Tier = 1,
        IconPath = Wind1Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Wind, 1 }
        }
    };

    public static readonly QuartzDefinition Evade2 = new()
    {
        Id = "evade_2",
        RewardSaveId = 1302,
        Tier = 2,
        IconPath = Wind2Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Wind, 3 }
        }
    };

    public static readonly QuartzDefinition Evade3 = new()
    {
        Id = "evade_3",
        RewardSaveId = 1303,
        Tier = 3,
        IconPath = Wind3Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Wind, 5 }
        }
    };

    public static readonly QuartzDefinition Cast1 = new()
    {
        Id = "cast_1",
        RewardSaveId = 2001,
        Tier = 1,
        IconPath = Time1Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Time, 2 },
            { Element.Mirage, 1 },
            { Element.Space, 1 }
        }
    };

    public static readonly QuartzDefinition Cast2 = new()
    {
        Id = "cast_2",
        RewardSaveId = 2002,
        Tier = 2,
        IconPath = Time2Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Time, 4 },
            { Element.Mirage, 2 },
            { Element.Space, 2 }
        }
    };

    public static readonly QuartzDefinition Cast3 = new()
    {
        Id = "cast_3",
        RewardSaveId = 2003,
        Tier = 3,
        IconPath = Time3Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Time, 6 },
            { Element.Mirage, 3 },
            { Element.Space, 3 }
        }
    };

    public static readonly QuartzDefinition EpCut1 = new()
    {
        Id = "ep_cut_1",
        RewardSaveId = 2101,
        Tier = 1,
        IconPath = Space1Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Space, 2 },
            { Element.Mirage, 1 },
            { Element.Time, 1 }
        }
    };

    public static readonly QuartzDefinition EpCut2 = new()
    {
        Id = "ep_cut_2",
        RewardSaveId = 2102,
        Tier = 2,
        IconPath = Space2Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Space, 4 },
            { Element.Mirage, 2 },
            { Element.Time, 2 }
        }
    };

    public static readonly QuartzDefinition EpCut3 = new()
    {
        Id = "ep_cut_3",
        RewardSaveId = 2103,
        Tier = 3,
        IconPath = Space3Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Space, 6 },
            { Element.Mirage, 3 },
            { Element.Time, 3 }
        }
    };

    public static readonly QuartzDefinition Ep1 = new()
    {
        Id = "ep_1",
        RewardSaveId = 2201,
        Tier = 1,
        IconPath = Mirage1Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Mirage, 2 },
            { Element.Space, 1 },
            { Element.Time, 1 }
        }
    };

    public static readonly QuartzDefinition Ep2 = new()
    {
        Id = "ep_2",
        RewardSaveId = 2202,
        Tier = 2,
        IconPath = Mirage2Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Mirage, 4 },
            { Element.Space, 2 },
            { Element.Time, 2 }
        }
    };

    public static readonly QuartzDefinition Ep3 = new()
    {
        Id = "ep_3",
        RewardSaveId = 2203,
        Tier = 3,
        IconPath = Mirage3Icon,
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Mirage, 6 },
            { Element.Space, 3 },
            { Element.Time, 3 }
        }
    };

    public static readonly IReadOnlyList<QuartzDefinition> All = new List<QuartzDefinition>
    {
        Defense1,
        Defense2,
        Defense3,

        Hp1,
        Hp2,
        Hp3,

        Attack1,
        Attack2,
        Attack3,

        Evade1,
        Evade2,
        Evade3,

        Cast1,
        Cast2,
        Cast3,

        EpCut1,
        EpCut2,
        EpCut3,

        Ep1,
        Ep2,
        Ep3
    };

    public static QuartzDefinition? GetById(string id)
    {
        // Temporary compatibility for older test data / commands.
        if (id == "guard_1")
            id = "defense_1";

        return All.FirstOrDefault(q => q.Id == id);
    }

    public static QuartzDefinition? GetByRewardSaveId(int rewardSaveId)
    {
        return All.FirstOrDefault(q => q.RewardSaveId == rewardSaveId);
    }

    public static IReadOnlyList<QuartzDefinition> GetByTier(int tier)
    {
        return All.Where(q => q.Tier == tier).ToList();
    }

    public static IReadOnlyList<QuartzDefinition> GetByTierWithFallback(int requestedTier)
    {
        for (var tier = requestedTier; tier >= 1; tier--)
        {
            var pool = GetByTier(tier);

            if (pool.Count > 0)
                return pool;
        }

        return All;
    }
}