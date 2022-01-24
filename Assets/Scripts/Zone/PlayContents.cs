using System.Collections.Generic;
using System.Linq;

public class PlayContents : ZoneContents
{
    // most recently installed part is first, to make removing quicker
    LinkedList<Card> orderedCards = new LinkedList<Card>(); 
    public PlayContents() : base(Zone.Play) {}
    public override void AddCard(Card card) {
        base.AddCard(card);
        orderedCards.AddFirst(card);
        CardExtension.UpdateExtensions();
    }
    public override void RemoveCard(Card card) {
        base.RemoveCard(card);
        orderedCards.Remove(card);
        CardExtension.UpdateExtensions();
    }
    public override Card[] GetCardsLeftToRight() {return orderedCards.Reverse().ToArray();}
    public override Card[] GetCards(){return orderedCards.ToArray();}
    public override int NumCards() {return orderedCards.Count;}
    public Card GetAbove(Card card) {
        var index = orderedCards.Find(card);
        if (index.Previous == null) return null;
        return index.Previous.Value;
    }
}
