using System.Collections.Generic;
using System.Linq;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public static class QuartzDatabase
{
    public static readonly QuartzDefinition Attack1 = new QuartzDefinition
    {
        Id = "attack_1",
        Tier = 1,
        IconPath = "res://TrailsWithinTheSpireMod/images/quartz/tier1_fire.png",
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Fire, 1 }
        }
    };

    public static readonly QuartzDefinition Guard1 = new QuartzDefinition
    {
        Id = "guard_1",
        Tier = 1,
        IconPath = "res://TrailsWithinTheSpireMod/images/quartz/tier1_earth.png",
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Earth, 1 }
        }
    };

    public static readonly QuartzDefinition Mind1 = new QuartzDefinition
    {
        Id = "mind_1",
        Tier = 1,
        IconPath = "res://TrailsWithinTheSpireMod/images/quartz/tier1_mirage.png",
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Mirage, 1 }
        }
    };

    public static readonly QuartzDefinition Hp1 = new QuartzDefinition
    {
        Id = "hp_1",
        Tier = 1,
        IconPath = "res://TrailsWithinTheSpireMod/images/quartz/tier1_water.png",
        ElementValues = new Dictionary<Element, int>
        {
            { Element.Water, 1 }
        }
    };

    public static List<QuartzDefinition> All => new()
    {
        Attack1,
        Guard1,
        Mind1,
        Hp1
    };

    public static QuartzDefinition? GetById(string id)
    {
        return All.FirstOrDefault(q => q.Id == id);
    }
}