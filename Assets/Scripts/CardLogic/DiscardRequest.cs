using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DiscardRequester {
    public int pendingRequests{get;private set;}
    public int pendingSelections{get;private set;}
    public Card requester;
    public static void RequestDiscard(Card card) {
        Game.S.discardRequester.pendingRequests++;
        if (!Game.S.ReversibleMode) {
            Game.S.discardRequester.requester = card;
            foreach (Card handCard in ZoneTracker.GetCards(Zone.Hand)) {
                if (handCard!=Game.S.discardRequester.requester) handCard.MakeSelectable();
            }
        }
    }
    public static void CancelRequest() {
        Game.S.discardRequester.pendingRequests--;
        Game.S.discardRequester.pendingSelections = 0;
        if (!Game.S.ReversibleMode) {
            foreach (Card handCard in ZoneTracker.GetCards(Zone.Hand)) {
                handCard.ClearSelectable();
            }
        }
    }
    public static void Select() {
        Game.S.discardRequester.pendingSelections++;
        if (Game.S.discardRequester.pendingSelections == Game.S.discardRequester.pendingRequests) {
            ProcessSelections();
        }
    }
    public static void CancelSelect() {Game.S.discardRequester.pendingSelections--;}
    public static void ProcessSelections() {
        foreach (Card card in ZoneTracker.GetCards(Zone.Hand)) {
            if (card.selected) {
                ZoneTracker.MoveCard(card,Zone.Hand,Zone.Junk);
                AnimationHandler.Animate(card,GameAction.Discarding);
                Game.S.discardRequester.requester.credits.Discard(card);
            }
            card.ClearSelectable();
        }
        Game.S.discardRequester.pendingSelections=0;
        Game.S.discardRequester.pendingRequests  =0;
        Game.GameStateChanged();
    }
}
