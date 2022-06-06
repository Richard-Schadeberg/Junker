using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// allows the player to select which cards they would like to discard to pay for a part's discard cost
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
                // requests are made while discarder is in hand, so don't make it selectable
                if (handCard!=S.requester) handCard.MakeSelectable();
            }
        }
    }
    public static void CancelRequest() {
        S.pendingRequests--;
        if (S.pendingRequests < 0) throw new Exception("Negative number of cards needed to discard");
        // no need to cancel selecting during temporary actions
        if (!Game.S.ReversibleMode) {
            S.pendingSelections = 0;
            foreach (Card handCard in ZoneTracker.GetCards(Zone.Hand)) handCard.ClearSelectable();
        }
    }
    // player has selected a card to discard
    // no need to know which one yet, it will be found by ProcessSelections()
    public static void Select() {
        S.pendingSelections++;
        // enough cards are selected to install the part
        if (S.pendingSelections == S.pendingRequests) {ProcessSelections();}
    }
    // player has unselected a card
    public static void CancelSelect() {S.pendingSelections--;}
    // enough cards are selected to install the part
    public static void ProcessSelections() {
        // find the selected cards and discard them
        foreach (Card card in ZoneTracker.GetCards(Zone.Hand)) {
            if (card.selected) {
                ZoneTracker.MoveCard(card,Zone.Hand,Zone.Junk);
                AnimationHandler.Animate(card,GameAction.Discarding);
                S.requester.credits.Discard(card);
            }
            // no more selecting cards
            card.ClearSelectable();
        }
        // installed part only outputs once discards have been selected
        InputOutput.Output(S.requester);
        S.pendingSelections=0;
        S.pendingRequests  =0;
        // selecting cards to discard is a player action
        Game.PlayerActionResolved();
    }
}
