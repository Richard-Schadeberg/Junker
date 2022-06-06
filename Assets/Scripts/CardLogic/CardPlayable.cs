using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// evaluate whether a card can be installed at this time
public static class CardPlayable {
    // does the playability of every card reflect the current gamestate?
    // ie. have they been evaluated since the last player action
    public static bool isValid = false;
    public static void PlayerActionResolved() {isValid = false;}
    public static void EvaluatePlayability() {
        // no need to evaluate if the data is still valid
        if (isValid) return;
        HashSet<Card> playableCards = new HashSet<Card>();
        // cards not in hand are unplayable, so set all cards to unplayable and then test the cards in hand
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
        // find which cards in hand can be played if another card (friend) is played first
        foreach (Card possiblyPlayable in handCards) {
            // no need to test cards that are already known to be playable
            if (playableCards.Contains(possiblyPlayable)) continue;
            foreach (Card playableFriend in playableCards) {
                // can the player install playableFriend, and then possiblyPlayable?
                if (CardInstall.CanInstallWith(possiblyPlayable,playableFriend)) {
                    possiblyPlayable.Playability = Playability.Almost;
                    // no need to keep testing this possiblyPlayable
                    break;
                }
            }
        }
        // set at end of loop to make unintended recursion more obvious
        isValid = true;
    }
}
