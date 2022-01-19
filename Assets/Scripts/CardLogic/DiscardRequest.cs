using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardRequest {
    public int pendingRequests{get;private set;}
    public Stack<Card> pendingRequesters = new Stack<Card>();
    public static void RequestDiscard(Card card) {Game.S.discardRequester._RequestDiscard(card);}
    public void _RequestDiscard(Card card) {
        pendingRequests++;
        if (!Game.S.ReversibleMode) {pendingRequesters.Push(card);}
    }
    public static void CancelRequests() {
        Game.S.discardRequester.pendingRequests = 0;
        Game.S.discardRequester.pendingRequesters.Clear();
    }
    public static void CancelRequest() {
        Game.S.discardRequester.pendingRequests--;
        Game.S.discardRequester.pendingRequesters.Pop();
    }
}
