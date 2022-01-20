using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits {
    Stack<Card> discards = new Stack<Card>();
    public void Discard(Card card) {discards.Push(card);}
    public void UndoDiscards() {while (discards.Count>0) UndoDiscard();}
    void UndoDiscard() {
        Card discarded = discards.Pop();
        ZoneTracker.MoveCard(discarded,Zone.Junk,Zone.Hand);
        DiscardRequester.RequestDiscard(null);
        if (!Game.S.ReversibleMode) {
            AnimationHandler.Animate(discarded,GameAction.DiscardReturn);
            Game.GameStateChanged();
        }
    }
}
