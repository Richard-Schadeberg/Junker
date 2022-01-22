using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// handles requesting discards from the game
public class DiscardRequester {
    // how many cards need to be discarded in order to resume the game
    // subtracted from cards in hand to determine how many cards are available for discarding
    public int pendingRequests{get;private set;}
    // how many cards have been selected so far
    public int pendingSelections{get;private set;}
    // the most recent card requesting discards
    public Card requester;
    // static pointer to the one instance of this object
    public static DiscardRequester S {get {return Game.S.discardRequester;}}
    public static void RequestDiscard(Card card) {
        S.pendingRequests++;
        // no selecting during temporary actions
        if (!Game.S.ReversibleMode) {
            S.requester = card;
            foreach (Card handCard in ZoneTracker.GetCards(Zone.Hand)) {
                if (handCard!=S.requester) handCard.MakeSelectable();
            }
        }
    }
    public static void CancelRequest() {
        S.pendingRequests--;
        // no need to cancel selecting during temporary actions
        if (!Game.S.ReversibleMode) {
            S.pendingSelections = 0;
            foreach (Card handCard in ZoneTracker.GetCards(Zone.Hand)) {
                handCard.ClearSelectable();
            }
        }
    }
    public static void Select() {
        S.pendingSelections++;
        if (S.pendingSelections == S.pendingRequests) {ProcessSelections();}
    }
    public static void CancelSelect() {S.pendingSelections--;}
    public static void ProcessSelections() {
        foreach (Card card in ZoneTracker.GetCards(Zone.Hand)) {
            if (card.selected) {
                ZoneTracker.MoveCard(card,Zone.Hand,Zone.Junk);
                AnimationHandler.Animate(card,GameAction.Discarding);
                S.requester.credits.Discard(card);
            }
            card.ClearSelectable();
        }
        S.pendingSelections=0;
        S.pendingRequests  =0;
        Game.GameStateChanged();
    }
}
