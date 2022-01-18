using System.Collections.Generic;
using System.Linq;

public class PlayContents : ZoneContents
{
    LinkedList<Card> orderedCards = new LinkedList<Card>(); // most recently installed part is first, to make removing quicker
    public PlayContents() : base(Zone.Play) {}
    public override void AddCard(Card card)
    {
        base.AddCard(card);
        orderedCards.AddFirst(card);
    }
    public override void RemoveCard(Card card)
    {
        base.RemoveCard(card);
        orderedCards.Remove(card);
    }
    public override Card[] GetCardsLeftToRight()
    {
        return (Card[])orderedCards.ToArray().Reverse();
    }
    public override int NumCards()
    {
        return orderedCards.Count;
    }
    public Card GetAbove(Card card) {
        var index = orderedCards.Find(card);
        if (index.Previous == null) return null;
        return index.Previous.Value;
    }
}
