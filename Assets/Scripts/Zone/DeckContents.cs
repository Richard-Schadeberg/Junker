using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeckContents : ZoneContents {
    // first card is top card of deck
    LinkedList<Card> orderedCards = new LinkedList<Card>();
    public DeckContents() : base(Zone.Deck) {}
    // adds card to top of deck (since undoing draws is the most common case)
    public override void AddCard(Card card) {
        base.AddCard(card);
        orderedCards.AddFirst(card);
    }
    // search starts from top of deck, so most efficient for deleting top of deck
    public override void RemoveCard(Card card) {
        base.RemoveCard(card);
        orderedCards.Remove(card);
    }
    public override Card[] GetCardsLeftToRight(){return orderedCards.ToArray();}
    public override Card[] GetCards(){return orderedCards.ToArray();}
    public override int NumCards() {return orderedCards.Count;}
    public void AddCardToBottom(Card card) {
        base.AddCard(card);
        orderedCards.AddLast(card);
    }
    public Card TopCard() { if (orderedCards.Count == 0) return null; else return orderedCards.First(); }
    public void MoveTopToBottom() { 
        if (orderedCards.Count != 0) {
            Card top = orderedCards.First();
            orderedCards.RemoveFirst();
            orderedCards.AddLast(top);
        } 
    }
}
