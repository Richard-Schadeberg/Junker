using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// tracks non-resource effects of cards (eg. draws, discards, altering other cards) so they can be reversed when the card is uninstalled
public class Credits {
    // wipe credits when turn ends to ensure player can't rewind to the previous turn
    public static void ClearCredits(Card card) {card.credits.ClearCredits();}
    public void ClearCredits() {
        discards.Clear();
        draws.Clear();
    }
    Stack<Card> discards = new Stack<Card>();
    Stack<Card> draws = new Stack<Card>();
    public void Discard(Card card) {discards.Push(card);}
    public void Draw(Card card) {draws.Push(card);}
    public void UndoDiscards() {while (discards.Count>0) UndoDiscard(discards.Pop());}
    public void UndoDraws() {while (draws.Count>0) UndoDraw(draws.Pop());}
    void UndoDiscard(Card discarded) {
        ZoneTracker.MoveCard(discarded,Zone.Junk,Zone.Hand);
        AnimationHandler.Animate(discarded,GameAction.DiscardReturn);
    }
    void UndoDraw(Card drawn) {
        ZoneTracker.MoveCard(drawn,Zone.Hand,Zone.Deck);
        AnimationHandler.Animate(drawn,GameAction.DrawReverse);
    }
}
