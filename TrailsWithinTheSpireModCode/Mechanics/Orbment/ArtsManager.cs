using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context; // Added for LocalContext
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;
using Godot; // Added for GD.Print
using System; // For Type
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Cards.Arts; // Added for Art card types
using MegaCrit.Sts2.Core.Combat; // Added for CombatState
using MegaCrit.Sts2.Core.Nodes.Rooms;
using TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment; // Added for NCombatRoom

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Singletons;

public class ArtsManager() : CustomSingletonModel(true, false)
{
    public override async Task BeforeCombatStart()
    {
        CombatState? combatState = CombatManager.Instance.DebugOnlyGetState();
        if (combatState == null) return;

        Player? player = combatState.Players.FirstOrDefault(p => LocalContext.IsMe(p));
        if (player == null) return;

        var artsPile = ArtsCardPile.ArtsPileType.GetPile(player);
        string[] initialArtIds = { "fire_bolt", "tear", "crest" };
        
        foreach (var id in initialArtIds)
        {
            try 
            {
                if (artsPile.Cards.Any(c => c.Id.ToString() == id)) continue;

                CardModel? canonicalCard = id switch
                {
                    "fire_bolt" => ModelDb.Card<FireBolt>(),
                    "tear" => ModelDb.Card<Tear>(),
                    "crest" => ModelDb.Card<Crest>(),
                    _ => null
                };

                if (canonicalCard != null)
                {
                    CardModel mutableCard = canonicalCard.ToMutable();
                    mutableCard.Owner = player;
                    await CardPileCmd.Add(mutableCard, ArtsCardPile.ArtsPileType);
                }
            }
            catch (Exception e)
            {
                GD.PrintErr($"ArtsManager: Error injecting {id} - {e.Message}");
            }
        }
    }
}
