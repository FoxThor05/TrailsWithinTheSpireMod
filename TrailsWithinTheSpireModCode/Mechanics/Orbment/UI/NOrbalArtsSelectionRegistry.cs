using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Models;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.UI;

public static class NOrbalArtsSelectionRegistry
{
    private static readonly Dictionary<NSimpleCardSelectScreen, HashSet<CardModel>> SelectableCardsByScreen = new();
    private static readonly Dictionary<NSimpleCardSelectScreen, HashSet<CardModel>> AllCardsByScreen = new();
    private static readonly Dictionary<CardModel, NSimpleCardSelectScreen> ScreenByCard = new();

    public static void Register(
        NSimpleCardSelectScreen screen,
        IEnumerable<CardModel> allCards,
        IEnumerable<CardModel> selectableCards)
    {
        Unregister(screen);

        var allSet = allCards.ToHashSet();
        var selectableSet = selectableCards.ToHashSet();

        AllCardsByScreen[screen] = allSet;
        SelectableCardsByScreen[screen] = selectableSet;

        foreach (var card in allSet)
            ScreenByCard[card] = screen;
    }

    public static void Unregister(NSimpleCardSelectScreen screen)
    {
        if (AllCardsByScreen.TryGetValue(screen, out var allCards))
        {
            foreach (var card in allCards)
            {
                if (ScreenByCard.TryGetValue(card, out var owningScreen) && owningScreen == screen)
                    ScreenByCard.Remove(card);
            }
        }

        AllCardsByScreen.Remove(screen);
        SelectableCardsByScreen.Remove(screen);
    }

    public static bool IsOrbalArtsScreen(NSimpleCardSelectScreen screen)
    {
        return AllCardsByScreen.ContainsKey(screen);
    }

    public static bool IsOrbalArtsCard(CardModel card)
    {
        return ScreenByCard.ContainsKey(card);
    }

    public static bool CanSelect(CardModel card)
    {
        if (!ScreenByCard.TryGetValue(card, out var screen))
            return true;

        if (!SelectableCardsByScreen.TryGetValue(screen, out var selectableCards))
            return false;

        return selectableCards.Contains(card);
    }

    public static bool IsDisabled(CardModel card)
    {
        return IsOrbalArtsCard(card) && !CanSelect(card);
    }
}