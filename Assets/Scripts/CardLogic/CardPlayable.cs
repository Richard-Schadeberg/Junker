using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// evaluate whether a card can be installed at this time
public static class CardPlayable {
    public static bool isValid = false;
    public static void GameStateChanged() {isValid = false;}
    public static void EvaluatePlayability() {
        List<Card> playableCards = new List<Card>();
        foreach (Card card in Game.S.cards) {
            card.Playability = Playability.Unplayable;
        }
        Card[] handCards = ZoneTracker.GetCards(Zone.Hand);
        foreach (Card card in handCards) {
            if (CardInstall.CanInstall(card)) {
                card.Playability = Playability.Playable;
                playableCards.Add(card);
            }
        }
        foreach (Card card in handCards) {
            if (playableCards.Contains(card)) continue;
            foreach (Card testCard in playableCards) {
                if (CardInstall.CanInstallWith(card,testCard)) {
                    card.Playability = Playability.Almost;
                }
            }
        }
        isValid = true;
    }
}
