using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using System;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Cards.Arts;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment;

public static class ArtCardFactory
{
    private static readonly Dictionary<string, Func<Player, CardModel>> Factories = new()
    {
        { "fire_bolt", player => player.Creature.CombatState.CreateCard<FireBolt>(player) },
        { "crest", player => player.Creature.CombatState.CreateCard<Crest>(player) },
        { "tear", player => player.Creature.CombatState.CreateCard<Tear>(player) }
    };

    public static bool TryCreate(string artId, Player player, out CardModel? card)
    {
        if (Factories.TryGetValue(artId, out var factory))
        {
            card = factory(player);
            return true;
        }

        card = null;
        return false;
    }
}