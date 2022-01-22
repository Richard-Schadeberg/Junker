using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// credit actions to cards so they can be reversed when the card is uninstalled
public class Credits {
    Stack<Card> discards = new Stack<Card>();
    Stack<Card> draws = new Stack<Card>();
    public void Discard(Card card) {discards.Push(card);}
    public void Draw(Card card) {draws.Push(card);}
    public void UndoCredits() {
        UndoDiscards();
        UndoDraws();
    }
    public void UndoDiscards() {while (discards.Count>0) UndoDiscard();}
    public void UndoDraws() {while (draws.Count>0) UndoDraw();}
    void UndoDiscard() {
        Card discarded = discards.Pop();
        ZoneTracker.MoveCard(discarded,Zone.Junk,Zone.Hand);
        AnimationHandler.Animate(discarded,GameAction.DiscardReturn);
        Game.GameStateChanged();
    }
    void UndoDraw() {
        Card drawn = draws.Pop();
        ZoneTracker.MoveCard(drawn,Zone.Hand,Zone.Deck);
        AnimationHandler.Animate(drawn,GameAction.DrawReverse);
        Game.GameStateChanged();
    }
}
