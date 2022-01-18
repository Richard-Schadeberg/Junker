using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Status {
	Playable=1,
	Almost=2,
	Unplayable=3
}
public static class CardPlayable {
    public static bool isValid = false;
    public static void GameStateChanged() {isValid = false;}
    public static void EvaluatePlayability() {
        List<Card> playableCards = new List<Card>();
        foreach (Card card in Game.S.cards) {
            card.Status = Status.Unplayable;
        }
        Card[] handCards = ZoneTracker.HandCards();
        foreach (Card card in handCards) {
            if (card.ImmediatelyPlayable()) {
                card.Status = Status.Playable;
                playableCards.Add(card);
            }
        }
        foreach (Card card in handCards) {
            foreach (Card testCard in playableCards) {
                if (card.PlayableWith(testCard)) {
                    card.Status = Status.Almost;
                }
            }
        }
        isValid = true;
    }
}
