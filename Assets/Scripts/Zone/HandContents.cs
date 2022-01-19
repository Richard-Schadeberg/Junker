using System.Linq;
using System.Collections.Generic;
public class HandContents : ZoneContents {
    public bool isSorted;
    Card[] sortedCards;
    public int availableDiscards {get; private set;}
    protected HashSet<Card> cards = new HashSet<Card>();
    public HandContents() : base(Zone.Hand) {isSorted = false;}
    public override void AddCard(Card card) {
        base.AddCard(card);
        cards.Add(card);
        isSorted = false;
        if (!card.noDiscard) availableDiscards++;
    }
    public override void RemoveCard(Card card) {
        base.RemoveCard(card);
        cards.Remove(card);
        isSorted = false;
        if (!card.noDiscard) availableDiscards--;
    }
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