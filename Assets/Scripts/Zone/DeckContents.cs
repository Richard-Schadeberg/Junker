using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeckContents : ZoneContents {
    LinkedList<Card> orderedCards = new LinkedList<Card>(); // first card is top card of deck
    public DeckContents() : base(Zone.Deck) {}
    public override void AddCard(Card card) {
        base.AddCard(card);
        orderedCards.AddFirst(card);
    }
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
    public Card DrawCard() {
        if (orderedCards.Count==0) return null;
        Card card = orderedCards.First();
        orderedCards.RemoveFirst();
        card.zone = Zone.Hand;
        packed = false;
        return card;
    }
}
