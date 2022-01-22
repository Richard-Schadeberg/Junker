using System.Linq;
using System.Collections.Generic;
public class HandContents : ZoneContents {
    protected HashSet<Card> cards = new HashSet<Card>();
    public HandContents() : base(Zone.Hand) {isSorted = false;}
    public override void AddCard(Card card) {
        base.AddCard(card);
        cards.Add(card);
        if (!Game.S.ReversibleMode) isSorted = false;
        if (!card.noDiscard) availableDiscards++;
    }
    public override void RemoveCard(Card card) {
        base.RemoveCard(card);
        cards.Remove(card);
        if (!Game.S.ReversibleMode) isSorted = false;
        if (!card.noDiscard) availableDiscards--;
    }
    // unlike other zones, hand needs to be sorted
    // this is computationally expensive, so it is not done unnecessarily
    public bool isSorted = false;
    Card[] sortedCards;
    public int availableDiscards {get; private set;}
    public override Card[] GetCardsLeftToRight() {
        if (isSorted) return sortedCards;
        else {
            sortedCards = cards.OrderBy(CardPriority.Priority).ToArray<Card>();
            isSorted=true;
            return sortedCards;
        }
    }
    public override int NumCards() {return cards.Count;}
    public override Card[] GetCards() {return cards.ToArray();}
}
